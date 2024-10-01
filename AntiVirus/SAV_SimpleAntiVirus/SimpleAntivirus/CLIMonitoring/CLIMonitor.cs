/**************************************************************************
 * Author:      Timothy Loh
 * Description: Main monitoring of CLI inputs (Registry read, write, create).
 * Last Modified: 11/08/24
 **************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;  // For dictionary usage
using System.Diagnostics;
using System.Management;  // Needed for getting the parent PID
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.GUI.Views.Pages;

namespace SimpleAntivirus.CLIMonitoring;

public class CompleteCopy
{
    public int ProcessID { get; set; }

    public DateTime TimeStamp { get; set; }

    public string KeyName { get; set; }

    public CompleteCopy(int processId, DateTime timeStamp, string keyName)
    {
        ProcessID = processId;
        TimeStamp = timeStamp;
        KeyName = keyName;
    }
}

public class CLIMonitor
{

    // Variable to keep track of total events checked
    private int _totalEventsChecked;

    // Dictionary to track registry paths and event counts
    private Dictionary<string, RegistrySummary> _registrySummary;

    private EventBus _eventBus;

    private Dictionary<int, string> _processCache;

    private ConcurrentQueue<CompleteCopy> _copyCreator;

    private DispatcherTimer _dispatcherTimer;

    private TraceEventSession _session;

    private Task _taskTracker;
    public CLIMonitor(EventBus eventBusPass)
    {
        // This will be relied upon to send alerts.
        _eventBus = eventBusPass;

        _processCache = new();

        _registrySummary = new();

        _totalEventsChecked = 0;

        _dispatcherTimer = new();
        _dispatcherTimer.Tick += dispatcherTimerEvent;
        _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);

        // Keep _quitEvent.set() for cancel maybe?

    }

    public void Setup()
    {
        _taskTracker = Task.Factory.StartNew(SetupAsync, TaskCreationOptions.LongRunning);
    }

    public void Cleanup()
    {
        if (_session != null)
        {
            _session.Source.StopProcessing();
        }
    }

    private void UpdateProcessCache()
    {
        _processCache.Clear();
        Process[] processArray = Process.GetProcesses();

        foreach (Process process in processArray)
        {
            _processCache.Add(process.Id, process.ProcessName);
        }
    }

    private void dispatcherTimerEvent(object sender, EventArgs e)
    {
        //System.Diagnostics.Debug.WriteLine("Updating cache");
        UpdateProcessCache();
    }

    private void SetupAsync()
    {

        UpdateProcessCache();
        _dispatcherTimer.Start();
        System.Diagnostics.Debug.WriteLine("Attempting to boot CLIMonitoring");
        string sessionName = "RegistryMonitorSession";
        _session = null;
        try
        {
            // Create the session without 'using' to control disposal manually
            _session = new TraceEventSession(sessionName)
            {
                StopOnDispose = true
            };

            // Enable Kernel Providers for registry-related events
            _session.EnableKernelProvider(KernelTraceEventParser.Keywords.Registry);  // Registry-related events only

            Debug.WriteLine("Kernel providers enabled successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing session: {ex.Message}");
            return;
        }

        // Subscribe to specific registry events
        _session.Source.Kernel.RegistrySetValue += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Set Value");
        };

        _session.Source.Kernel.RegistryOpen += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Open Key");
        };

        _session.Source.Kernel.RegistryCreate += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Create Key");
        };

        _session.Source.Kernel.RegistryDelete += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Delete Key");
        };

        _session.Source.Kernel.RegistryQueryValue += async (RegistryTraceData data) =>
        {
            //CompleteCopy copy = new(data.ProcessID, data.TimeStamp, data.KeyName);
            //Dispatcher dis = Dispatcher.FromThread(Thread.CurrentThread);
            await TrackRegistryEvent(data, "Query Value");
        };

        // Start processing events in a background task
        try
        {
            Debug.WriteLine("Processing events...");
            _session.Source.Process();
            System.Diagnostics.Debug.WriteLine("CLI MONITORING ENDED HERE");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in event processing: {ex.Message}");
        }
    }


    private async Task TrackRegistryEvent(RegistryTraceData data, string action)
    {

        try
        {
            // Increment the total events checked
            Interlocked.Increment(ref _totalEventsChecked);

            // Get the process name by PID
            string processName = GetProcessNameById(data.ProcessID);

            // Filter events by target process name (powershell, cmd, pwsh)
            if (!IsRelevantProcess(processName))
            {
                return;  // Ignore events not related to PowerShell or CMD
            }

            // Get the registry path
            string registryPath = data.KeyName;


            // Send Alert
            await _eventBus.PublishAsync("Terminal Scanning", "Informational", $"{processName} {action} registry {registryPath}", "Lookout for suspicious activity");

            // If the path is already tracked, update the existing entry

            if (_registrySummary.ContainsKey(registryPath))
            {
                _registrySummary[registryPath].IncrementAction(action, data.TimeStamp, processName, data.ProcessID);
            }
            else
            {
                // Add new registry path with the current action
                _registrySummary[registryPath] = new RegistrySummary(registryPath, processName, data.ProcessID);
                _registrySummary[registryPath].IncrementAction(action, data.TimeStamp, processName, data.ProcessID);

            }
        }
        catch (Exception ex)
        {
            // Suppress any errors but log them if needed for debugging
            Debug.WriteLine($"Error tracking registry event: {ex.Message}");
        }
    }









    // Output a summary of the events and clear the dictionary after output
    private void OutputSummary()
    {
        if (_registrySummary.Count > 0)
        {
            Debug.WriteLine("\nRegistry Path Access Summary:");
            foreach (var entry in _registrySummary)
            {
                Debug.WriteLine(entry.Value.GetHighLevelSummary());
            }

            // Clear the registry summary after outputting to ensure only new events are tracked
            _registrySummary.Clear();
        }
        else
        {
            Debug.WriteLine("No new registry events were captured in the last 5 seconds.");
        }
    }









    // Filter relevant processes (PowerShell or CMD)
    private bool IsRelevantProcess(string processName)
    {
        string[] relevantProcesses = { "powershell", "pwsh", "cmd" };
        foreach (string relevantProcess in relevantProcesses)
        {
            if (processName.ToLower().Contains(relevantProcess))
            {
                return true;
            }
        }
        return false;
    }

    // Get the name of the process by PID
    private string GetProcessNameById(int pid)
    {
        try
        {
            if (_processCache.ContainsKey(pid))
            {
               return _processCache[pid];
            }

            //return "N/A";
            //using (Process proc = Process.GetProcessById(pid))
            //{
            //   return proc.ProcessName;
            //}
        }
        catch
        {
            // If the process can't be found, return null silently
        }
        return "N/A";
    }
}













// Class to summarize registry operations for each path
class RegistrySummary
{
    public string RegistryPath { get; private set; }
    public string ProcessName { get; private set; } // Track process name
    public int ProcessID { get; private set; } // Track process ID
    public int SetValueCount { get; private set; }
    public int OpenKeyCount { get; private set; }
    public int CreateKeyCount { get; private set; }
    public int DeleteKeyCount { get; private set; }
    public int QueryValueCount { get; private set; }
    public DateTime LastAccessTime { get; private set; } // Track the last access time for the latest event

    public RegistrySummary(string registryPath, string processName, int processID)
    {
        RegistryPath = registryPath;
        ProcessName = processName;
        ProcessID = processID;
    }

    // Increment action counters and update last access time
    public void IncrementAction(string action, DateTime eventTime, string processName, int processID)
    {
        LastAccessTime = eventTime;  // Update last access time on every action
        ProcessName = processName;   // Update process name on every action (in case it changes)
        ProcessID = processID;       // Update process ID on every action
        switch (action)
        {
            case "Set Value":
                SetValueCount++;
                break;
            case "Open Key":
                OpenKeyCount++;
                break;
            case "Create Key":
                CreateKeyCount++;
                break;
            case "Delete Key":
                DeleteKeyCount++;
                break;
            case "Query Value":
                QueryValueCount++;
                break;
        }
    }
















    // Get a high-level summary of the registry path's actions
    public string GetHighLevelSummary()
    {
        // Build the action summary only for non-zero counts
        string actionSummary = $"PID: {ProcessID}, Process Name: {ProcessName}, Actions: ";
        List<string> actions = new List<string>();

        if (SetValueCount > 0) actions.Add($"Set Value: {SetValueCount}");
        if (OpenKeyCount > 0) actions.Add($"Open Key: {OpenKeyCount}");
        if (CreateKeyCount > 0) actions.Add($"Create Key: {CreateKeyCount}");
        if (DeleteKeyCount > 0) actions.Add($"Delete Key: {DeleteKeyCount}");
        if (QueryValueCount > 0) actions.Add($"Query Value: {QueryValueCount}");

        // If there are no actions, return empty string (no output)
        if (actions.Count == 0)
        {
            return string.Empty;
        }

        actionSummary += string.Join(", ", actions);

        // Second line: Time and Registry Key Path
        string timeAndPath = $"Time: {LastAccessTime}, Registry Path: {RegistryPath}";

        // Return the combined result
        return $"{actionSummary}\n{timeAndPath}";
    }
}

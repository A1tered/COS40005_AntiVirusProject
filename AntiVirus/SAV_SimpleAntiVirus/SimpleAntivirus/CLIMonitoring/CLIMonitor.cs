/**************************************************************************
 * Author:      Timothy Loh
 * Description: Main monitoring of CLI inputs (Registry read, write, create).
 * Last Modified: 11/08/24
 **************************************************************************/

using System;
using System.Collections.Generic;  // For dictionary usage
using System.Diagnostics;
using System.Management;  // Needed for getting the parent PID
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.CLIMonitoring;
public class CLIMonitor
{

    // Variable to keep track of total events checked
    private int _totalEventsChecked;

    // Dictionary to track registry paths and event counts
    private Dictionary<string, RegistrySummary> _registrySummary;

    private EventBus _eventBus;

    private Task _taskTracker;
    public CLIMonitor(EventBus eventBusPass)
    {
        // This will be relied upon to send alerts.
        _eventBus = eventBusPass;

        _registrySummary = new();

        _totalEventsChecked = 0;

        // Keep _quitEvent.set() for cancel maybe?

    }

    private void SetupAsync()
    {
        System.Diagnostics.Debug.WriteLine("Attempting to boot CLIMonitoring");
        string sessionName = "RegistryMonitorSession";
        TraceEventSession session = null;
        try
        {
            // Create the session without 'using' to control disposal manually
            session = new TraceEventSession(sessionName)
            {
                StopOnDispose = true
            };

            // Enable Kernel Providers for registry-related events
            session.EnableKernelProvider(KernelTraceEventParser.Keywords.Registry);  // Registry-related events only

            Console.WriteLine("Kernel providers enabled successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing session: {ex.Message}");
            return;
        }

        // Subscribe to specific registry events
        session.Source.Kernel.RegistrySetValue += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Set Value");
        };

        session.Source.Kernel.RegistryOpen += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Open Key");
        };

        session.Source.Kernel.RegistryCreate += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Create Key");
        };

        session.Source.Kernel.RegistryDelete += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Delete Key");
        };

        session.Source.Kernel.RegistryQueryValue += async (RegistryTraceData data) =>
        {
            await TrackRegistryEvent(data, "Query Value");
        };

        // Start processing events in a background task
        try
        {
            Console.WriteLine("Processing events...");
            session.Source.Process();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in event processing: {ex.Message}");
        }
    }

    public void Setup()
    {
        _taskTracker = Task.Run(() => SetupAsync());
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
            Console.WriteLine($"Error tracking registry event: {ex.Message}");
        }
    }









    // Output a summary of the events and clear the dictionary after output
    private void OutputSummary()
    {
        if (_registrySummary.Count > 0)
        {
            Console.WriteLine("\nRegistry Path Access Summary:");
            foreach (var entry in _registrySummary)
            {
                Console.WriteLine(entry.Value.GetHighLevelSummary());
            }

            // Clear the registry summary after outputting to ensure only new events are tracked
            _registrySummary.Clear();
        }
        else
        {
            Console.WriteLine("No new registry events were captured in the last 5 seconds.");
        }
    }









    // Filter relevant processes (PowerShell or CMD)
    private bool IsRelevantProcess(string processName)
    {
        string[] relevantProcesses = { "powershell", "pwsh", "cmd", "windowsterminal" };
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
            using (Process proc = Process.GetProcessById(pid))
            {
                return proc.ProcessName;
            }
        }
        catch
        {
            // If the process can't be found, return null silently
            return "N/A";
        }
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

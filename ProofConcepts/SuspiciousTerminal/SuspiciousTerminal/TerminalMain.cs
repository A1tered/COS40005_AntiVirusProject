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

class Program
{
    // ManualResetEvent to keep the application running
    static ManualResetEvent _quitEvent = new ManualResetEvent(false);

    // Variable to keep track of total events checked
    static int totalEventsChecked = 0;

    // Dictionary to track registry paths and event counts
    static Dictionary<string, RegistrySummary> registrySummary = new Dictionary<string, RegistrySummary>();

    static void Main()
    {
        Console.WriteLine("Program started.");

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

        Console.WriteLine("Listening for registry events...");

        // Start continuous task to output the analysis every 5 seconds
        Task.Run(async () =>
        {
            while (!_quitEvent.WaitOne(0))
            {
                await Task.Delay(5000);  // Wait for 5 seconds
                OutputSummary();
            }
        });

        // Subscribe to specific registry events
        session.Source.Kernel.RegistrySetValue += (RegistryTraceData data) =>
        {
            TrackRegistryEvent(data, "Set Value");
        };

        session.Source.Kernel.RegistryOpen += (RegistryTraceData data) =>
        {
            TrackRegistryEvent(data, "Open Key");
        };

        session.Source.Kernel.RegistryCreate += (RegistryTraceData data) =>
        {
            TrackRegistryEvent(data, "Create Key");
        };

        session.Source.Kernel.RegistryDelete += (RegistryTraceData data) =>
        {
            TrackRegistryEvent(data, "Delete Key");
        };

        session.Source.Kernel.RegistryQueryValue += (RegistryTraceData data) =>
        {
            TrackRegistryEvent(data, "Query Value");
        };

        // Start processing events in a background task
        Task.Run(() =>
        {
            try
            {
                Console.WriteLine("Processing events...");
                session.Source.Process();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in event processing: {ex.Message}");
            }
        });

        // Keep the application running until the user presses Ctrl+C
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            _quitEvent.Set();
        };

        Console.WriteLine("Press Ctrl+C to exit...");
        _quitEvent.WaitOne();

        // Dispose of the session when done
        try
        {
            session.Dispose();
        }
        catch
        {
            // Suppress any disposal errors
        }

        Console.WriteLine("Program exiting.");
    }










    static void TrackRegistryEvent(RegistryTraceData data, string action)
    {
        try
        {
            // Increment the total events checked
            Interlocked.Increment(ref totalEventsChecked);

            // Get the process name by PID
            string processName = GetProcessNameById(data.ProcessID);

            // Filter events by target process name (powershell, cmd, pwsh)
            if (!IsRelevantProcess(processName))
            {
                return;  // Ignore events not related to PowerShell or CMD
            }

            // Get the registry path
            string registryPath = data.KeyName;

            // If the path is already tracked, update the existing entry
            if (registrySummary.ContainsKey(registryPath))
            {
                registrySummary[registryPath].IncrementAction(action, data.TimeStamp, processName, data.ProcessID);
            }
            else
            {
                // Add new registry path with the current action
                registrySummary[registryPath] = new RegistrySummary(registryPath, processName, data.ProcessID);
                registrySummary[registryPath].IncrementAction(action, data.TimeStamp, processName, data.ProcessID);
            }
        }
        catch (Exception ex)
        {
            // Suppress any errors but log them if needed for debugging
            Console.WriteLine($"Error tracking registry event: {ex.Message}");
        }
    }









    // Output a summary of the events and clear the dictionary after output
    static void OutputSummary()
    {
        if (registrySummary.Count > 0)
        {
            Console.WriteLine("\nRegistry Path Access Summary:");
            foreach (var entry in registrySummary)
            {
                Console.WriteLine(entry.Value.GetHighLevelSummary());
            }

            // Clear the registry summary after outputting to ensure only new events are tracked
            registrySummary.Clear();
        }
        else
        {
            Console.WriteLine("No new registry events were captured in the last 5 seconds.");
        }
    }









    // Filter relevant processes (PowerShell or CMD)
    static bool IsRelevantProcess(string processName)
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
    static string GetProcessNameById(int pid)
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

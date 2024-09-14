/**************************************************************************
 * Author:      Timothy Loh
 * Description: Main monitoring of CLI inputs.
 * Last Modified: 11/08/24
 **************************************************************************/

using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

class Program
{
    // ManualResetEvent to keep the application running
    static ManualResetEvent _quitEvent = new ManualResetEvent(false);

    // Variable to keep track of total events checked
    static int totalEventsChecked = 0;

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

            // Enable the Kernel Registry provider
            session.EnableKernelProvider(KernelTraceEventParser.Keywords.Registry);

            Console.WriteLine("Session enabled successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing session: {ex.Message}");
            return;
        }

        Console.WriteLine("Listening for registry events...");

        // Start a task to output the totalEventsChecked every 5 seconds
        Task.Run(async () =>
        {
            while (!_quitEvent.WaitOne(0))
            {
                Console.WriteLine($"Total events checked: {totalEventsChecked}");
                await Task.Delay(5000);
            }
        });

        // Subscribe to kernel events
        session.Source.Kernel.All += (TraceEvent data) =>
        {
            try
            {
                // Increment the total events checked
                Interlocked.Increment(ref totalEventsChecked);

                // Log only registry events
                if (data.ProviderName == "Microsoft-Windows-Kernel-Registry")
                {
                    // Get the process ID from the event data
                    int pid = data.ProcessID;
                    string processName = GetProcessNameById(pid);

                    if (!string.IsNullOrEmpty(processName))
                    {
                        // Check if the process is a terminal-related process (cmd, powershell, etc.)
                        if (IsTerminalProcess(processName))
                        {
                            // Log only terminal-related events
                            Console.WriteLine($"Terminal Registry Event: {data.EventName}, PID: {pid}, Process Name: {processName}");
                        }
                        else
                        {
                            Console.WriteLine($"Skipping non-terminal process: {processName} (PID: {pid})");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not retrieve process name for PID {pid}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in event handler: {ex.Message}");
            }
        };


        // Start processing events in a background task
        Task.Run(() =>
        {
            Console.WriteLine("Event processing task started.");
            try
            {
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
        session.Dispose();

        Console.WriteLine("Program exiting.");
    }

    static bool IsTerminalProcess(string processName)
    {
        if (string.IsNullOrEmpty(processName)) return false;

        // Check if the process name contains terminal-related names like cmd or powershell
        var terminalProcessNames = new[]
        {
        "cmd", "powershell", "pwsh", "windowsterminal"
    };

        // Ensure the process name contains one of the terminal process names
        foreach (var name in terminalProcessNames)
        {
            if (processName.ToLowerInvariant().Contains(name))
            {
                return true;
            }
        }

        return false;
    }

    static string GetProcessNameById(int pid)
    {
        try
        {
            using (Process proc = Process.GetProcessById(pid))
            {
                return proc.ProcessName;
            }
        }
        catch (Exception ex)
        {
            // Log and return null if process cannot be found
            Console.WriteLine($"Error retrieving process name for PID {pid}: {ex.Message}");
            return null;
        }
    }
}

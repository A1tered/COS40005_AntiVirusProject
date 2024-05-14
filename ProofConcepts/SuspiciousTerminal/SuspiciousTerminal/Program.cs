using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await MonitorCmdAndPowershellAsync();
        //"await" "async" and "task" is part of asynchornous programming
        // "async" defines this method as asynchornous
        //"Await" defines the process that is to be waited on. The control goes back to the controller, so whoever called this method. 
        // "task" defines that output.
    }

    static async Task MonitorCmdAndPowershellAsync()
    {
        //Because this is always monitoring, the while loop never ends. Thanks to async programming, this does not hold up the rest of the code.
        while (true)
        {
            // Get all processes that are named "cmd" or "powershell"
            var processes = Process.GetProcesses()
                                   .Where(p => p.ProcessName.Equals("cmd", StringComparison.OrdinalIgnoreCase) ||p.ProcessName.Equals("powershell", StringComparison.OrdinalIgnoreCase) ||p.ProcessName.Equals("pwsh", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var process in processes)
            {
                // Log the process details
                DetectedTerminalisRunning(process);
            }

            await Task.Delay(5000); // Wait for 5 seconds before checking again. Delay needed to reduce hardware usage while maintaining performance
        }
    }

    static void DetectedTerminalisRunning(Process process)
    {
        // This method describes what the program does when it detects a cmd or powershell running
        // It should immediately deploy windows hooks. For later implementation.
        Console.WriteLine($"Suspicious activity detected in process: {process.ProcessName}, PID: {process.Id}, Start Time: {process.StartTime}");
    }



}
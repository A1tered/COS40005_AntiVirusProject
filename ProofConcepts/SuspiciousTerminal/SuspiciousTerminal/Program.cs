using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await MonitorCmdAndPowershellAsync();
    }

    static async Task MonitorCmdAndPowershellAsync()
    {
        while (true)
        {
            // Get all processes that are named "cmd" or "powershell"
            var processes = Process.GetProcesses()
                                   .Where(p => p.ProcessName.Equals("cmd", StringComparison.OrdinalIgnoreCase) ||
                                               p.ProcessName.Equals("powershell", StringComparison.OrdinalIgnoreCase) ||
                                               p.ProcessName.Equals("pwsh", StringComparison.OrdinalIgnoreCase))
                                   .ToList();

            foreach (var process in processes)
            {
                // Log the process details
                ReportSuspiciousActivity(process);
            }

            await Task.Delay(5000); // Wait for 5 seconds before checking again
        }
    }

    static void ReportSuspiciousActivity(Process process)
    {
        // Print an alert to the console
        Console.WriteLine($"Suspicious activity detected in process: {process.ProcessName}, PID: {process.Id}, Start Time: {process.StartTime}");
    }



}
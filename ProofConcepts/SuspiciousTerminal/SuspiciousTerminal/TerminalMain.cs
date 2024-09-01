/**************************************************************************
 * Author:      Timothy Loh
 * Description: Main monitoring of CLI inputs. 
 * - Behaviour of keyboard inputs
 * - Behaviour of mouse movements
 * Last Modified: 11/08/24
 **************************************************************************/


using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DatabaseProofOfConcept;
using System.Net.NetworkInformation;
using Microsoft.Data.Sqlite;

class Program
{
    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    // The hook ID and the hook procedure callback
    private static IntPtr _hookID = IntPtr.Zero;
    private static LowLevelMouseProc _proc = HookCallback;

    // P/Invoke declarations for hook-related functions
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

   

    //-------------------------------------------------------------------------------------------------------------------------






    static Task Main(string[] args)
    {

        //Variable will hold the state of any foreign network connections. If checks determine a remote connection is occuring, Value is changed to True.
        // bool TerminalNetworkConnection = false;

        //Initialise();
        // MonitorCLIAsync();

        //"await" "async" and "task" is part of asynchornous programming
        // "async" defines this method as asynchornous
        //"Await" defines the process that is to be waited on. The control goes back to the controller, so whoever called this method. 
        // "task" defines that output.

        while (true)
        {
            CheckRemoteConnections();
        }

        //MonitorMouseMovements();
    }


    static async Task MonitorMouseMovements()
    {
        // Set the mouse hook
        _hookID = SetWindowsHookEx(14, _proc, IntPtr.Zero, 0);
        Console.WriteLine("Monitoring mouse movements. Press Ctrl+C to exit...");

        // Start asynchronous monitoring
        await MonitorMouseMovementsAsync();
    }

    private static async Task MonitorMouseMovementsAsync()
    {
        // Continuously run to keep the hook active
        while (true)
        {
            await Task.Delay(100); // Introduce a small delay to prevent high CPU usage
        }
    }
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (int)wParam == 0x0200) // WM_MOUSEMOVE
        {
            // Retrieve mouse coordinates from lParam
            int x = Marshal.ReadInt32(lParam);
            int y = Marshal.ReadInt32(lParam, 4);
            Console.WriteLine($"X = {x}, Y = {y}");
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }




    static async Task MonitorCLIAsync()
    {
        //Because this is always monitoring, the while loop never ends. Thanks to async programming, this does not hold up the rest of the code.
        while (true)
        {
            // Get all processes that are named "cmd" or "powershell"

            var processes = Process.GetProcesses()
                                   .Where(p => p.ProcessName.Equals("cmd", StringComparison.OrdinalIgnoreCase) || p.ProcessName.Equals("powershell", StringComparison.OrdinalIgnoreCase) || p.ProcessName.Equals("pwsh", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var process in processes)
            {
                // Log the process details
                DetectedTerminalisRunning(process);
            }

            await Task.Delay(5000); // Wait for 5 seconds before checking again. Delay needed to reduce hardware usage while maintaining performance
        }
    }

    static async Task MonitorCLIkeyboardAsync()
    {
        //Monitors and analyses the keyboard inputs into CLI. 
        //This will record inputs and commands. 

        //Compare inputs, mark accordingly to a criteria. 
    }


    static void DetectedTerminalisRunning(Process process)
    {
        // This method describes what the program does when it detects a cmd or powershell running
        // It should immediately deploy windows hooks. For later implementation.
        Console.WriteLine($"Suspicious activity detected in process: {process.ProcessName}, PID: {process.Id}, Start Time: {process.StartTime}");
    }

    static void CheckDatabases()
    {
        //This method runs to check databases do exist. Mainly used on initialisation.


    }

    static void CheckRemoteConnections()
    {
        // This method will check and record any remote sessions, SSH, Telnet etc.
        // Alert if network connection found. 
        //If time, offer option to block remote connection.

        var ActiveTcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
        //Get all active TCP Connections, store each object  in an array. 

        int[] PortsOfInterest = { 22, 23 };
        //Used later to tell the checker, which ports to monitor. We are only monitoring ssh and tcp connections. Most common ports being 22 and 23.

        var remoteConnections = ActiveTcpConnections.Where(connection => PortsOfInterest.Contains(connection.RemoteEndPoint.Port) && connection.State == TcpState.Established).ToList();
        //Checks each connection object, imports ports to monitor via "PortsOfInterest", compares those values to the connection object port value. Adds objects that match to the list variable "RemoteConnections"


        if (remoteConnections.Any())
        {
            Console.WriteLine("Remote connections detected:");
            foreach (var connection in remoteConnections)
            {
                Console.WriteLine($"Remote Address: {connection.RemoteEndPoint.Address}, Port: {connection.RemoteEndPoint.Port}");
            }
        }
        else
        {
            Console.WriteLine("No remote SSH or Telnet connections detected.");
        }
    }


    static void Initialise()
    {
        //Create Database for the entire Suspicious Terminal Monitoring
        
        Database TerminalDB = new Database();
        //Mouse Table Column Names
        String[] MouseColumnnames = { "DateTime", "XLocation", "YLocation" };
        //Mouse Table Column Data Types
        //Windows mouse data from Windows Hook recieved in Point Structure, Made of X int and Y int.
        String[] MouseColumnType = { "DateTime", "Int", "Int" };
        //Create Table for Mouse Data
        TerminalDB.CreateTable("MouseMovement", MouseColumnnames, MouseColumnType);




        //Keyboard Table Column Names
        String[] KeyboardColumnnames = { "DateTime", "Key Pressed", "Session" };
        //Keyboard Table Column Data Types
        String[] KeyboardColumnType = { "DateTime", "String", "String" };
        //Create Table for Keyboard Data
        TerminalDB.CreateTable("KeyboardEvents", KeyboardColumnnames, KeyboardColumnType);



        //NetworkConnections Table Column Names
        String[] NetworkConColumnnames = { "DateTime", "Port", "External IP" };
        //NetworkConnections Table Column Data Types
        String[] NetworkConColumnType = { "DateTime", "Int", "String" };
        //Create Table for Keyboard Data
        TerminalDB.CreateTable("KeyboardEvents", KeyboardColumnnames, KeyboardColumnType);

    }

    static void SendAlert(int Type, string[] Details)
    {
        //Handles all alert events. Gets info of type of alert and details. formats and calls Alert functionality.



    }

}


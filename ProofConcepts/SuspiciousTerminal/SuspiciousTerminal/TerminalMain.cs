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
using System.Net.NetworkInformation;
using Microsoft.Data.Sqlite;
using DatabaseFoundations;
using System.Threading;
class Program
{


    //-------------------------------------------------------------------------------------------------------------------------


    static void Main(string[] args)
    {

        while (true)
        {
            CheckRemoteConnections();
            Thread.Sleep(10);
        }

      
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
        var activeTcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

        Console.WriteLine("All active TCP connections:");
        foreach (var connection in activeTcpConnections)
        {
            Console.WriteLine($"Local: {connection.LocalEndPoint.Address}:{connection.LocalEndPoint.Port} -> Remote: {connection.RemoteEndPoint.Address}:{connection.RemoteEndPoint.Port}, State: {connection.State}");
        }
    }



    static void Initialise()
    {
        SuspiciousTerminalDatabaseIntermediary suspiciousTerminalDatabaseIntermediary = new SuspiciousTerminalDatabaseIntermediary("SusTerminalDB", true);
        //Creates new database intermediary instance



    }

    static void SendAlert(int Type, string[] Details)
    {
        //Handles all alert events. Gets info of type of alert and details. formats and calls Alert functionality.



    }

}


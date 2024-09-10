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
class Program
{


    //-------------------------------------------------------------------------------------------------------------------------


    static Task Main(string[] args)
    {

        //Initialise();

        while (true)
        {
            CheckRemoteConnections();
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
        // This method will check and record any remote sessions, SSH, Telnet etc.
        // Alert if network connection found. 
        //If time, offer option to block remote connection.

        var ActiveTcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
        //Get all active TCP Connections, store each object  in an array. 

        int[] PortsOfInterest = { 22, 23};
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
        SuspiciousTerminalDatabaseIntermediary suspiciousTerminalDatabaseIntermediary = new SuspiciousTerminalDatabaseIntermediary("SusTerminalDB", true);
        //Creates new database intermediary instance



    }

    static void SendAlert(int Type, string[] Details)
    {
        //Handles all alert events. Gets info of type of alert and details. formats and calls Alert functionality.



    }

}


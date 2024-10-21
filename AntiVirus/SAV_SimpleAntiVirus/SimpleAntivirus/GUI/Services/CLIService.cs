/**************************************************************************
 * File:        CLIService.cs
 * Author:      Christopher Thompson 
 * Description: Sets up Timothy Loh's CLI monitoring code to run in background.
 * Last Modified: 21/10/2024
 **************************************************************************/

using SimpleAntivirus.Alerts;
using SimpleAntivirus.CLIMonitoring;

namespace SimpleAntivirus.GUI.Services
{
    public class CLIService
    {
        private CLIMonitor _cliMonitor;
        public CLIService(EventBus eventbus)
        {
            _cliMonitor = new(eventbus);
        }

        public void Setup()
        {
            //Dispatcher dis = Dispatcher.FromThread(Thread.CurrentThread);
            _cliMonitor.Setup();
        }

        public void Remove()
        {
            _cliMonitor.Cleanup();
        }
    }
}

using SimpleAntivirus.Alerts;
using SimpleAntivirus.CLIMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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

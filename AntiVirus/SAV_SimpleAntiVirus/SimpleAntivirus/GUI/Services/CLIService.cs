using SimpleAntivirus.Alerts;
using SimpleAntivirus.CLIMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAntivirus.GUI.Services
{
    public class CLIService
    {
        private CLIMonitor _cliMonitor;
        public CLIService(EventBus eventbus)
        {
            _cliMonitor = new(eventbus);
        }
    }
}

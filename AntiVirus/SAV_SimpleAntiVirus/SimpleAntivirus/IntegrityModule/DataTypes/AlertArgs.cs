using SimpleAntivirus.IntegrityModule.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.IntegrityModule.DataTypes
{
    public class AlertArgs : EventArgs
    {
        public Alert AlertSet { get; set; }
    }
}

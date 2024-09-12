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
        public string Component { get; set; } = "";
        public string Severity { get; set; } = "";
        public string Message { get; set; } = "";
        public string SuggestedAction { get; set; } = "";
    }
}

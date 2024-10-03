using IntegrityModule.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.DataTypes
{
    public class AlertArgs : EventArgs
    {
        public Alert AlertSet { get; set; }
    }
}

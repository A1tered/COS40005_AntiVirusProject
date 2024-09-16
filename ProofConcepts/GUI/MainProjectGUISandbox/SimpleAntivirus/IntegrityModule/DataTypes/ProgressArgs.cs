using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAntivirus.IntegrityModule.DataTypes
{
    public class ProgressArgs : EventArgs
    {
        public float Progress { get; set; }
        public string ProgressInfo { get; set; } = "";
    }
}

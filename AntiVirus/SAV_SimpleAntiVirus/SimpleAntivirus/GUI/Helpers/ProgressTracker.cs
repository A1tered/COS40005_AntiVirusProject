using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAntivirus.GUI.Helpers
{
    public class ProgressTracker
    {
        public long CurrentSize { get; set; }
        public long TotalSize { get; set; }
        public int ItemsScanned { get; set; }
        public int ThreatsFound { get; set; }
        public double Progress;

        public double GetProgress()
        {
            Progress = (CurrentSize / TotalSize) * 100;
            return Progress;
        }
    }
}
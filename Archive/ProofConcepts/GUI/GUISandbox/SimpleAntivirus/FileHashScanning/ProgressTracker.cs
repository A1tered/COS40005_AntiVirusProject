using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAntivirus.FileHashScanning
{
    public class ProgressTracker
    {
        public long totalSize;
        private long _currentSize;
        private readonly object _lock = new object();

        public ProgressTracker(long totalSize)
        {
            this.totalSize = totalSize;
            this._currentSize = 0;
        }

        public void UpdateTracker(long size)
        {
            lock (_lock)
            {
                _currentSize += size;
                Debug.WriteLine($"Current size: {_currentSize} bytes");
                double progress = (_currentSize / (double)totalSize) * 100;
                Debug.WriteLine($"{progress}% complete");
            }
        }
    }
}

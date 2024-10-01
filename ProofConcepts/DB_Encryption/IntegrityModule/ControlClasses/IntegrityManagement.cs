using DatabaseFoundations;
using IntegrityModule.IntegrityComparison;
using IntegrityModule.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.ControlClasses
{
    public class IntegrityManagement
    {
        private IntegrityConfigurator _integrityConfigurator;
        private IntegrityCycler _integrityCycler;
        private ReactiveControl _reactiveControl;
        public IntegrityManagement(IntegrityDatabaseIntermediary integrityIntermediary)
        {
            _integrityConfigurator = new IntegrityConfigurator(integrityIntermediary);
            _integrityCycler = new IntegrityCycler(integrityIntermediary);
            _reactiveControl = new(integrityIntermediary);
            //_reactiveControl.Initialize(); (Uncomment when ready)
        }

        // Alert Handler to be placed here at later date.

        /// <summary>
        /// Start scanning all data in database, and compare to system files.
        /// </summary>
        /// <param name="benchmark">Whether to return debug time taken for scan</param>
        /// <returns></returns>
        public bool Scan(bool benchmark = false)
        {
            Stopwatch timer = new();
            if (benchmark)
            {
                timer.Start();
            }
            bool returnItem = _integrityCycler.InitiateScan();
            if (benchmark)
            {
                timer.Stop();
                Console.WriteLine($"Time taken for scan: {timer.Elapsed}");
            }
            return returnItem;
        }

        public bool AddBaseline(string path, bool debug = false)
        {
           return _integrityConfigurator.AddIntegrityDirectory(path, debug);
        }

        public bool RemoveBaseline(string path)
        {
            return _integrityConfigurator.RemoveIntegrityDirectory(path);
        }

        public bool ClearDatabase()
        {
            return _integrityConfigurator.RemoveAll();
        }

        public void ChangeSetAmount(int amount)
        {
            _integrityCycler.AmountSet = amount;
        }

        public void Close()
        {
            _integrityConfigurator.Close();
        }
    }
}

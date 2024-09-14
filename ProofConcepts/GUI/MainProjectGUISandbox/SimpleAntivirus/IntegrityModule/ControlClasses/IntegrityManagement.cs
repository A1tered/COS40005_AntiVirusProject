/**************************************************************************
 * File:        IntegrityManagement.cs
 * Author:      Christopher Thompson, etc.
 * Description: Initiate scans, provide functions for outer module callers.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.Alerts;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;
using SimpleAntivirus.IntegrityModule.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.IntegrityModule.Db;

namespace SimpleAntivirus.IntegrityModule.ControlClasses
{
    public class IntegrityManagement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IntegrityConfigurator _integrityConfigurator;
        private IntegrityCycler _integrityCycler;
        private ReactiveControl _reactiveControl;
        private float _progress;
        private float _addProgress;
        private string _progressInfo;
        public IntegrityManagement(IntegrityDatabaseIntermediary integrityIntermediary)
        {
            _integrityConfigurator = new IntegrityConfigurator(integrityIntermediary);
            ViolationHandler tempHandler = new();
            integrityIntermediary.DataAddProgress += ProgressUpdateAddHandler;
            tempHandler.AlertFlag += AlertHandler;
            _integrityCycler = new IntegrityCycler(integrityIntermediary, tempHandler);
            _integrityCycler.ProgressUpdate += ProgressUpdateHandler;
            _reactiveControl = new(integrityIntermediary, _integrityCycler);
            _progressInfo = "";
        }

        public void StartReactiveControl()
        {
            _reactiveControl.Initialize();
        }

        private void AlertHandler(object? sender, AlertArgs alertInfo)
        {
            Console.WriteLine("Alert Handler Event Triggered Successfully");
        }


        // Alert Handler to be placed here at later date.

        /// <summary>
        /// Start scanning all data in database, and compare to system files.
        /// </summary>
        /// <param name="benchmark">Whether to return debug time taken for scan</param>
        /// <returns></returns>
        public async Task<List<IntegrityViolation>> Scan(bool benchmark = false)
        {
            return await _integrityCycler.InitiateScan();
        }

        // Get how many pages would exist.
        public int GetPages()
        {
            return _integrityConfigurator.GetPageAmount();
        }

        // Get a page of entries, to be utilised by GUI. Each page will return about 10 sets. 
        public Dictionary<string, string> BaselinePage(int page)
        {
            return _integrityConfigurator.GetPage(page);
        }

        /// <summary>
        /// Add path to integrity database
        /// </summary>
        /// <param name="path">Directory</param>
        /// <param name="debug">Whether to send info to console</param>
        /// <returns></returns>
        public async Task<bool> AddBaseline(string path, bool debug = false)
        {
           bool success = await _integrityConfigurator.AddIntegrityDirectory(path, debug);
            if (success)
            {
                // If integrity items were successfully added, then add to reactive control. (As it was not initialized with the database).
                _reactiveControl.Add(path);
            }
            return success;
        }

        /// <summary>
        /// Remove path from integrity database (Execution of this function should be moderated)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool RemoveBaseline(string path)
        {
            return _integrityConfigurator.RemoveIntegrityDirectory(path);
        }

        /// <summary>
        /// Remove all contents of database, via the IntegrityTrack table.
        /// Function calls to this should be moderated.
        /// </summary>
        /// <returns></returns>
        public bool ClearDatabase()
        {
            return _integrityConfigurator.RemoveAll();
        }

        /// <summary>
        /// Amount of items in each Asynchronous set.
        /// </summary>
        /// <param name="amount">Amount of items in a set, must be positive.</param>
        public void ChangeSetAmount(int amount)
        {
            _integrityCycler.AmountSet = amount;
        }

        private void ProgressUpdateHandler(object? sender, ProgressArgs progressData)
        {
            Progress = progressData.Progress;
            ProgressInfo = progressData.ProgressInfo;
            //Console.Write($"Progress: {Progress}");
            //Console.Write("\r");
        }

        private void ProgressUpdateAddHandler(object? sender, ProgressArgs progressData)
        {
            AddProgress = Math.Min(progressData.Progress, 100);
            //Console.Write($"Progress: {Progress}");
            //Console.Write("\r");
        }

        public float AddProgress

        {
            get
            {
                return _addProgress;
            }
            set
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("AddProgress"));
                _addProgress = value;
            }
        }

        public float Progress

        {
            get
            {
                return _progress;
            }
            set
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
                _progress = value;
            }
        }

        public string ProgressInfo
        {
            get
            {
                return _progressInfo;
            }
            set
            {
                _progressInfo = value;
            }
        }
    }
}

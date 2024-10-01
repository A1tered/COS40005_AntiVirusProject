/**************************************************************************
 * File:        IIntegrityManagement.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interface for: Initiate scans, provide functions for outer module callers.
 * Last Modified: 29/09/2024
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
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.IntegrityModule.Interface
{
    public interface IIntegrityManagement : INotifyPropertyChanged
    {

        public Task<bool> StartReactiveControl();


        // Alert Handler to be placed here at later date.

        /// <summary>
        /// Start scanning all data in database, and compare to system files.
        /// </summary>
        /// <param name="benchmark">Whether to return debug time taken for scan</param>
        /// <returns></returns>
        public Task<List<IntegrityViolation>> Scan(bool benchmark = false);

        // Get how many pages would exist.
        public int GetPages();

        // Get a page of entries, to be utilised by GUI. Each page will return about 10 sets. 
        public Dictionary<string, string> BaselinePage(int page);

        /// <summary>
        /// Add path to integrity database
        /// </summary>
        /// <param name="path">Directory</param>
        /// <param name="debug">Whether to send info to console</param>
        /// <returns></returns>
        public Task<bool> AddBaseline(string path, bool debug = false);

        /// <summary>
        /// Remove path from integrity database (Execution of this function should be moderated)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool RemoveBaseline(string path);

        /// <summary>
        /// Remove all contents of database, via the IntegrityTrack table.
        /// Function calls to this should be moderated.
        /// </summary>
        /// <returns></returns>
        public bool ClearDatabase();

        public Task CleanUp();

        /// <summary>
        /// Amount of items in each Asynchronous set.
        /// </summary>
        /// <param name="amount">Amount of items in a set, must be positive.</param>
        public void ChangeSetAmount(int amount);



        public double AddProgress { get; set; }

        public double Progress { get; set; }

        public string ProgressInfo { get; set; }

        public EventBus EventSocket { get; set; }
    }
}

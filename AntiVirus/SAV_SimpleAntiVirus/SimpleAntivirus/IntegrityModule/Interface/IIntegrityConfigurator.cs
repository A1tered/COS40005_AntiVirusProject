/**************************************************************************
 * File:        IIntegrityConfigurator.cs
 * Author:      Christopher Thompson, etc.
 * Description: A interface for a simplified interface for the database.
 * Last Modified: 29/09/2024
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.IntegrityModule.Db;

namespace SimpleAntivirus.IntegrityModule.Interface
{
    public interface IIntegrityConfigurator
    {
        /// <summary>
        /// Add windows directory / file to Integrity Database
        /// </summary>
        /// <param name="path">Windows directory.</param>
        /// <param name="debug">Whether console information is shown</param>
        /// <returns></returns>
        public Task<bool> AddIntegrityDirectory(string path, bool debug = false);

        /// <summary>
        /// Remove a certain path (item or folder) from IntegrityDatabase
        /// </summary>
        /// <param name="path">Windows directory</param>
        /// <returns></returns>
        public bool RemoveIntegrityDirectory(string path);

        // If -1, then return all pages.
        public Dictionary<string, string> GetPage(int page);
        public int GetPageAmount();

        /// <summary>
        /// Remove all items within IntegrityTrack table.
        /// </summary>
        /// <returns></returns>
        public bool RemoveAll();

        // Cancel ongoing operations (Just adding to database)
        public Task CancelOperations();
    }
}

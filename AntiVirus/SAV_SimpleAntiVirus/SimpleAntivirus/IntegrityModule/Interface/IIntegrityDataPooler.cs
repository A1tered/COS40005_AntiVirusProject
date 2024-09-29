/**************************************************************************
 * File:        IIntegrityDataPooler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interface for: Interface for the purpose to Scan a set of database directories and compares them with system documents.
 * Last Modified: 29/09/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpleAntivirus.IntegrityModule.IntegrityComparison
{
    public interface IIntegrityDataPooler
    {

        /// <summary>
        /// Retrieving a set of the database, compare with system files.
        /// </summary>
        /// <returns>Violations found via mismatching Hashes.</returns>
        public Task<List<IntegrityViolation>> CheckIntegrity(CancellationToken ct);

        /// <summary>
        /// Singular comparison between the database and a windows file.
        /// </summary>
        /// <returns>Violation or Null</returns>
        public Task<List<IntegrityViolation>> CheckIntegrityDirectory();

        public int Set { get; }
    }
}

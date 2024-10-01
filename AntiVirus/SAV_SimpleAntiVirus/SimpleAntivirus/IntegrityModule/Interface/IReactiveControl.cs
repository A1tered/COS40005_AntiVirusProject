/**************************************************************************
 * File:        IIntegrityConfigurator.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interface for: Deals with scans that are reactive, such that if a file change occurs that it scans that certain file for performance, and the reduction
 * of time taken to respond to issues.
 * Last Modified: 29/09/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;
using SimpleAntivirus.IntegrityModule.Db;
using System.IO;

namespace SimpleAntivirus.IntegrityModule.Interface
{
    public interface IReactiveControl
    {
        public bool Initialize();

        public Task Add(string path);

        public void Remove(string path);

        public void RemoveAll();
    }
}

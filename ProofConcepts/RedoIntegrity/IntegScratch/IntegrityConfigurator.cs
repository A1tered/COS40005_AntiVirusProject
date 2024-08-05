using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityCheckingFromScratch
{
    public class IntegrityConfigurator
    {
        private IntegrityDatabaseIntermediary _databaseHandler;

        public IntegrityConfigurator(IntegrityDatabaseIntermediary databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }

        // Add integrity to database
        public bool AddIntegrityDirectory(string fileDirectory)
        {
            double datetimeGenerate = new DateTimeOffset(Directory.GetLastWriteTime(fileDirectory)).ToUnixTimeSeconds();
            // time of signature generation
            double nowTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            string hashGeneration = Hasher.HashFile(fileDirectory);
            double sizeBytes = new FileInfo(fileDirectory).Length;
            if (hashGeneration != null)
            {
                return _databaseHandler.AddBaseline(fileDirectory, hashGeneration, datetimeGenerate, nowTime, sizeBytes);
            }
            return false;
        }

        public bool RemoveIntegrityDirectory(string fileDirectory)
        {
           return _databaseHandler.RemoveBaseline(fileDirectory);
        }

        public bool ClearIntegrityDatabase()
        {
            return _databaseHandler.ClearDatabase();
        }
    }
}

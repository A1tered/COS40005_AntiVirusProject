//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SimpleAntivirus.FileHashScanning
//{
//    public class Configurator
//    {
//        DatabaseConnector _databaseConnector;
//        public Configurator(string database)
//        {
//            _databaseConnector = new(database, true);
//        }

//        public void AddHashToDatabase(string directory)
//        {
//            if (_databaseConnector.AddHash(new Hasher().OpenHashFile(directory)))
//            {
//                // Add code to show that hash is added.

//                // "Added hash to database"
//            }
//            else
//            {
//                // Add code to show that hash is already added or failed to be added.

//                //"Hash not added, likely already added."
//            }
//        }
//    }
//}

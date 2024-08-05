using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityCheckingFromScratch
{
    public class IntegrityDatabaseIntermediary : DatabaseHandler
    {
        public IntegrityDatabaseIntermediary(string databaseName) : base(databaseName)
        {

        }

        // Likely deprecated if replace keyword in command works optimally.
        private bool CheckExistenceOfEntry(string fileDirectory)
        {
            SqliteCommand commandCreation = new();
            commandCreation.CommandText = @"
                SELECT directory FROM IntegrityTrack WHERE directory = $directorySet
            ";
            commandCreation.Parameters.AddWithValue("$directorySet", fileDirectory);
            SqliteDataReader existenceReader = ExecuteReturnQuery(commandCreation);
            if (existenceReader.HasRows)
            {
                // Entry is already within database.
                return true;
            }
            return false;
        }

        public bool AddBaseline(string fileDirectory, string hashGeneration, double datetimeGenerate, double nowTime, double sizeBytes)
        {
            SqliteCommand commandCreation = new();
            // last modification time
            if (hashGeneration != null)
            {
                if (CheckExistenceOfEntry(fileDirectory))
                {
                    //Console.WriteLine("WARNING : Entry is being overwritten");
                }
                commandCreation.CommandText = @"
            REPLACE INTO IntegrityTrack values ($fileDirectory, $hashGeneration, $datetimeGenerate, $nowTime, $sizeBytes)
            ";
                commandCreation.Parameters.AddWithValue("$fileDirectory", fileDirectory);
                commandCreation.Parameters.AddWithValue("$hashGeneration", hashGeneration);
                commandCreation.Parameters.AddWithValue("$datetimeGenerate", datetimeGenerate);
                commandCreation.Parameters.AddWithValue("$nowTime", nowTime);
                commandCreation.Parameters.AddWithValue("$sizeBytes", sizeBytes);
                return ExecuteNonReturnQuery(commandCreation);
            }
            return false;
        }

        public bool RemoveBaseline(string fileDirectory)
        {
            SqliteCommand commandCreation = new();
            // last modification time

                commandCreation.CommandText = @"
            DELETE FROM IntegrityTrack WHERE directory = $directorySpecific
            ";
            commandCreation.Parameters.AddWithValue("$directorySpecific", fileDirectory);
            return ExecuteNonReturnQuery(commandCreation);
        }

        // Dangerous command
        public bool ClearDatabase()
        {
            SqliteCommand commandCreation = new();
            // last modification time

            commandCreation.CommandText = @"
            DELETE FROM IntegrityTrack
            ";
            return ExecuteNonReturnQuery(commandCreation);
        }


        public int AssessAmountRows()
        {
            SqliteCommand commandCreation = new();
            Dictionary<string, string> returnDictionary = new();
            commandCreation.CommandText = @"
            SELECT COUNT(*) FROM IntegrityTrack
            ";
            SqliteDataReader dataReader = ExecuteReturnQuery(commandCreation);
            dataReader.Read();
            return dataReader.GetInt32(0);
        }

        public List<string> QueryDirectory(string directory)
        {
            SqliteCommand commandCreation = new();
            List<string> returnSet = new();
            commandCreation.CommandText = @"
                SELECT * FROM IntegrityTrack WHERE directory = $directorySet
            ";
            commandCreation.Parameters.AddWithValue("$directorySet", directory);
            SqliteDataReader unpackReader = ExecuteReturnQuery(commandCreation);
            if (unpackReader.HasRows)
            {
                // Entry is already within database.
                unpackReader.Read();
                for (int i = 0; i < unpackReader.FieldCount; i++)
                {
                    returnSet.Add(unpackReader.GetString(i));
                }
            }
            return returnSet;
        }

        public Dictionary<string, string> QueryDirectoryHashPairs(int set, int amountHandledPerSet = 100)
        {
            SqliteCommand commandCreation = new();
            Dictionary<string, string> returnDictionary = new();
            int setUp = set + 1;
            commandCreation.CommandText = @"
            SELECT * FROM IntegrityTrack LIMIT $limit OFFSET $offset
            ";
            Console.WriteLine($"limit: {amountHandledPerSet}");
            Console.WriteLine($"offset: {(setUp - 1) * amountHandledPerSet}");
            commandCreation.Parameters.AddWithValue("$limit", amountHandledPerSet);
            commandCreation.Parameters.AddWithValue("$offset", (setUp - 1) * amountHandledPerSet);
            SqliteDataReader dataReader = ExecuteReturnQuery(commandCreation);
            int amount = 0;
            while (dataReader.Read())
            {
                // Directory -> Hash
                returnDictionary[dataReader.GetString(0)] = dataReader.GetString(1);
                amount++;
            }
            return returnDictionary;
        }



        public string RequestDebugDisplay(string command = @"SELECT * FROM IntegrityTrack")
        {
            SqliteCommand commandCreation = new();
            commandCreation.CommandText = command;
            SqliteDataReader dataReader = ExecuteReturnQuery(commandCreation);
            List<List<string>> fabricateTable = new();
            fabricateTable.Add(new List<string>() {"Directory", "Hash", "ModificationTime", "Signature Creation", "OriginalSize"});
            while (dataReader.Read())
            {
                List<string> tempRow = new();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    // Byte to highest
                    if (i == 4)
                    {
                        tempRow.Add(DisplayHandler.ByteToSize(long.Parse(dataReader.GetString(i))));
                    }
                    else
                    {
                        tempRow.Add(dataReader.GetString(i));
                    }
                }
                fabricateTable.Add(tempRow);
            }
            return DisplayHandler.StringListToStringDisplay(fabricateTable);
        }
    }
}

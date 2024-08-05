using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFoundations
{
    public class IntegrityDatabaseIntermediary : DatabaseIntermediary
    {
        public IntegrityDatabaseIntermediary(string databaseName) : base(databaseName)
        {

        }

        public Tuple<string, string, long, long, long> GetDirectoryInfo(string directory)
        {
            SqliteCommand command = new();
            command.CommandText = "SELECT * FROM IntegrityTrack WHERE directory = $directorySet";
            command.Parameters.AddWithValue("$directorySet", directory);
            SqliteDataReader dataReader = QueryReader(command);
            if (dataReader.Read())
            {
                return new Tuple<string, string, long, long, long>(dataReader.GetString(0), dataReader.GetString(1), dataReader.GetInt64(2), dataReader.GetInt64(3), dataReader.GetInt64(4));
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, string> GetSetEntries(int set, int amountHandledPerSet)
        {
            SqliteCommand command = new();
            Dictionary<string, string> returnDictionary = new();
            command.CommandText = @"SELECT * FROM IntegrityTrack LIMIT $limit OFFSET $offset";
            int setUp = set + 1;
            command.Parameters.AddWithValue("$limit", amountHandledPerSet);
            command.Parameters.AddWithValue("$offset", (setUp - 1) * amountHandledPerSet);
            SqliteDataReader dataReader = QueryReader(command);
            int amount = 0;
            while (dataReader.Read())
            {
                // Directory -> Hash
                returnDictionary[dataReader.GetString(0)] = dataReader.GetString(1);
                amount++;
            }
            return returnDictionary;
        }

    }
}

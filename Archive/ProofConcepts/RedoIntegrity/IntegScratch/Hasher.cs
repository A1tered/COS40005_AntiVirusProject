using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityCheckingFromScratch
{
    public class Hasher
    {
        public static string HashFile(string directory)
        {
            if (Path.Exists(directory))
        {
            StringBuilder hashReturn = new();
                using (FileStream openFile = File.Open(directory, FileMode.Open, FileAccess.Read))
                {
                    byte[] returnByteSet = SHA256.HashData(openFile);
                    foreach (byte individualByte in returnByteSet)
                    {
                        hashReturn.Append(individualByte.ToString("X2"));
                    }
                    return hashReturn.ToString();
                }
                
            }
            return "";
        }

        // Return Modification Time / File Size
        public static List<long> FileInfoUnpack(string directory)
        {
           List<long> returnInfoSet = new();
           FileInfo fileInfoRepresent = new FileInfo(directory);
           returnInfoSet.Add(new DateTimeOffset(fileInfoRepresent.LastWriteTime).ToUnixTimeSeconds());
           returnInfoSet.Add(fileInfoRepresent.Length);
           return returnInfoSet;
        }
    }
}

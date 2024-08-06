using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DatabaseFoundations.IntegrityRelated
{
    public static class FileInfoRequester
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

        public static Tuple<long, long> RetrieveFileInfo(string path)
        {
            FileInfo fileInfoCreate = new FileInfo(path);
            return new Tuple<long, long>(new DateTimeOffset(fileInfoCreate.LastWriteTime).ToUnixTimeSeconds(), fileInfoCreate.Length);
        }
    }
}

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
        /// <summary>
        /// Hash File via SHA256
        /// </summary>
        /// <param name="directory">File Directory</param>
        /// <returns>SHA256 Hash of file</returns>
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

        /// <summary>
        /// Get certain file info from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Tuple(long, long) LastWriteTime, Size of file in bytes</returns>
        public static Tuple<long, long> RetrieveFileInfo(string path)
        {
            FileInfo fileInfoCreate = new FileInfo(path);
            return new Tuple<long, long>(new DateTimeOffset(fileInfoCreate.LastWriteTime).ToUnixTimeSeconds(), fileInfoCreate.Length);
        }
    }
}

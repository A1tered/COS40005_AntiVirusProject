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
                try
                {
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
                catch (IOException e )
                {
                    Console.WriteLine($"IOException (Likely process use) {directory}");
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine($"Unauthorized access {directory}");
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


        public static List<string> PathCollector(string path)
        {
            List<string> pathProcess = new();
            Queue<string> directoryProcess = new();
            // If for any process to access status, or for debug console:
            string tempPathUnpack = "";
            if (Directory.Exists(path))
            {
                Directory.GetDirectories(path).ToList().ForEach(directoryProcess.Enqueue);
                // Item is directory, so process contents
                Directory.GetFiles(path).ToList<string>().ForEach(pathProcess.Add);
                while (directoryProcess.Count() > 0 && pathProcess.Count() < 10000)
                {
                    tempPathUnpack = directoryProcess.Dequeue();
                    try
                    {
                        Directory.GetDirectories(tempPathUnpack).ToList().ForEach(directoryProcess.Enqueue);
                        Directory.GetFiles(tempPathUnpack).ToList<string>().ForEach(pathProcess.Add);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Unauthorized Permission Warning: {tempPathUnpack}");
                    }
                }
            }
            else
            {
                pathProcess.Add(path);
            }
            return pathProcess;
        }

        /// <summary>
        /// Private function that finds files/directories with certain names, within the provided directory.
        /// </summary>
        /// <param name="startDirectory">Name of database SQLite file</param>\
        /// <param name="term">Search term for what the desired file path should have</param>
        public static string FileDirectorySearcher(string startDirectory, string term)
        {
            string[] filePaths = Directory.GetFiles(startDirectory).Concat(Directory.GetDirectories(startDirectory)).ToArray();
            // Find path that contains database.
            foreach (string path in filePaths)
            {
                if (path.Contains(term))
                {
                    return path;
                }
            }
            return null;
        }
    }
}

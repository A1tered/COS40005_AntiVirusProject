/**************************************************************************
 * File:        FileInfoRequester.cs
 * Author:      Christopher Thompson, etc.
 * Description: Provides useful static functions that can be used by other modules, such functions include Hashing, size bytes to Size label, retrieving file info
 * Last Modified: 8/10/2024
 **************************************************************************/

using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SimpleAntivirus.IntegrityModule.DataRelated
{
    public static class FileInfoRequester
    {


        // String is shortened eg. "Hello my name is jack" -> "...is jack"
        public static string TruncateString(string itemCandidate, int lengthValue = 40)
        {
            if (itemCandidate.Length > lengthValue)
            {
                string redoString = "...";
                int startPoint = itemCandidate.Length - lengthValue;
                redoString = redoString + itemCandidate.Substring(startPoint, lengthValue);
                return redoString;
            }
            return itemCandidate;
        }

        public static string SizeValueToLabel(long bytes)
        {
            float byteChange = bytes;
            int counter = 0;
            string[] label = { "B", "KB", "MB", "GB", "TB" };
            while (Math.Abs(byteChange / 1024) >= 1)
            {
                byteChange /= 1024;
                counter++;
            }
            double roundChange = Math.Round(byteChange, 2);
            return $"{roundChange} {label[counter]}";
        }

        /// <summary>
        /// Hash File via SHA1
        /// </summary>
        /// <param name="directory">File Directory</param>
        /// <returns>SHA1 Hash of file, 
        /// if error then returns "" (empty string)
        /// if empty file then returns "empty" (notes that a file should be empty)
        /// </returns>
        /// 

        public static async Task<string> HashFile(string directory)
        {
            if (Path.Exists(directory))
            {
                StringBuilder hashReturn = new();
                try
                {
                    using (FileStream openFile = new(directory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                    {
                        byte[] returnByteSet = await SHA1.HashDataAsync(openFile);
                        foreach (byte individualByte in returnByteSet)
                        {
                            hashReturn.Append(individualByte.ToString("X2"));
                        }
                        return hashReturn.ToString();
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine($"Directory not found: {directory} (Privilege issues?)");
                }
                catch (EndOfStreamException e)
                {
                    System.Diagnostics.Debug.WriteLine($"End of stream exception: {directory}");
                }
                catch (FileNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine($"File not found: {directory} (Privilege issues?)");
                }
                catch (FileLoadException e)
                {
                    System.Diagnostics.Debug.WriteLine($"File failed to load");
                }
                catch (UnauthorizedAccessException e)
                {
                    System.Diagnostics.Debug.WriteLine($"Unauthorized access {directory}");
                }
                catch (IOException e)
                {
                    System.Diagnostics.Debug.WriteLine($"Generic IO exception {directory}");
                    // Potential issue if it is 0 bytes.
                    if (Path.Exists(directory))
                    {
                        if (new FileInfo(directory).Length == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"IO exception dealt with, empty file determined (cannot be hashed)");
                            return "empty";
                        }
                    }
                }

            }
            return "";
        }


        /// <summary>
        /// Hashes items as a set, with an attempt to get better performance.
        /// </summary>
        /// <param name="directorySet"></param>
        /// <returns></returns>
        public static async Task<List<string>> HashSet(List<string> directorySet)
        {
            List<string> returnedHashes = new();
            List<Task<string>> hashConverterTasks = new();

            foreach (string directory in directorySet) {
                hashConverterTasks.Add(HashFile(directory));
            }
            // when all converted
            await Task.WhenAll(hashConverterTasks.ToArray());
            foreach (Task<string> taskString in hashConverterTasks)
            {
                returnedHashes.Add(await taskString);
            }
            return returnedHashes;
        }

        /// <summary>
        /// Get certain file info from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Tuple(long, long) LastWriteTime, Size of file in bytes</returns>
        public static Tuple<long, long> RetrieveFileInfo(string path)
        {
            try
            {
                FileInfo fileInfoCreate = new FileInfo(path);
                return new Tuple<long, long>(new DateTimeOffset(fileInfoCreate.LastWriteTime).ToUnixTimeSeconds(), fileInfoCreate.Length);
            }
            catch (FileNotFoundException e)
            {
                return new Tuple<long, long>(0,0);
            }
        }

        /// <summary>
        /// Get all paths in a directory.
        /// </summary>
        /// <param name="path">Windows Directory Path.</param>
        /// <returns>All window paths to items within that directory and sub directories.</returns>
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
                        System.Diagnostics.Debug.WriteLine($"Unauthorized Permission Warning: {tempPathUnpack}");
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

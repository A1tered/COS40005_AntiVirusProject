/**************************************************************************
 * File:        Hasher.cs
 * Author:      Joel Parks
 * Description: Hashes files
 * Last Modified: 21/10/2024
 **************************************************************************/


using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SimpleAntivirus.FileHashScanning
{
    /// <summary>
    /// Responsible for opening files and hashing their contents.
    /// </summary>
    public class Hasher
    {
        public Hasher()
        {

        }

        public string OpenHashFile(string directory)
        {
            try
            {
                using (FileStream openedFile = File.Open(directory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    return HashFile(openedFile);
                }
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                return ("Non-readable");
            }
        }

        public string[] OpenHashFileBatch(string[] directorySet)
        {
            List<String> hashReturn = new(); // Size is equal to directorySet
            List<Task> taskTrack = new();
            foreach (string directory in directorySet)
            {
                try
                {
                    taskTrack.Add(Task.Run(() => OpenHashFile(directory)));
                }
                catch (IOException error)
                {
                    
                }
            }
            Task.WhenAll(taskTrack.ToArray());
            foreach (Task<string> task in taskTrack)
            {
                hashReturn.Add(task.Result);
            }
            return hashReturn.ToArray();
        }

        public string HashFile(FileStream fileStream)
        {
            StringBuilder stringBuild = new();
            byte[] byteArray = SHA1.HashData(fileStream);
            foreach (byte byteRep in byteArray)
            {
                stringBuild.Append(byteRep.ToString("X2"));
            }
            fileStream.Close();
            return stringBuild.ToString();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FindTheHash
{
    /// <summary>
    /// Responsible for opening files and hashing their contents. (It is a simple class that is utilised thoroughly)
    /// </summary>
    public class Hasher
    {
        public Hasher()
        {

        }

        public static FileStream OpenFile()
        {
            Console.WriteLine("File Selection\n");
            Console.WriteLine("Please enter the path of the file: ");
            string path = Console.ReadLine();
            if (path != "")
            {
                try
                {
                    FileStream currentFile = File.Open(path, FileMode.Open, FileAccess.Read);
                    return currentFile;
                }
                catch (IOException error)
                {
                    return null;
                }
            }
            Console.Clear();
            return null;
        }

        public string OpenHashFile(string directory)
        {
            try
            {
                using (FileStream openedFile = File.Open(directory, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            return stringBuild.ToString().ToLower();
        }
        
    }
}

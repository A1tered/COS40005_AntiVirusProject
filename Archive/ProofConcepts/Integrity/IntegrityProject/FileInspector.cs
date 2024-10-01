using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FindTheHash
{
    public class FileInspector
    {
        public FileInspector()
        {

        }

        public Tuple<long, long, long> GetIntegrityAttributes(string directory)
        {
            try
            {
                DateTime datetimeObject = File.GetLastWriteTime(directory);
                long timeModification = new DateTimeOffset(datetimeObject).ToUnixTimeSeconds();
                DateTime currentTime = DateTime.Now;
                long signatureCreation = new DateTimeOffset(currentTime).ToUnixTimeSeconds();
                long sizeBytes = new FileInfo(directory).Length;
                return new Tuple<long, long, long>(timeModification, signatureCreation, sizeBytes);
            }
            finally
            {
            }
        }

        public string OpenHashFile(string directory)
        {
            try
            {
                string hashReturn = "";
                if (File.Exists(directory))
                {
                    using (FileStream openedFile = File.Open(directory, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        hashReturn = HashFile(openedFile);
                    }
                    return hashReturn;
                }
                return ("N");
            }
            catch (FileNotFoundException error)
            {
                return ("N");
            }
            catch (IOException error)
            {
                return ("Non-readable");
            }
        }

        public string HashFile(FileStream fileStream)
        {
            StringBuilder stringBuild = new();
            byte[] byteArray = SHA256.HashData(fileStream);
            foreach (byte byteRep in byteArray)
            {
                stringBuild.Append(byteRep.ToString("X2"));
            }
            return stringBuild.ToString();
        }
        
    }
}

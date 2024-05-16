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

        public Tuple<long, long> GetIntegrityAttributes(string directory)
        {
            try
            {
                DateTime datetimeObject = File.GetLastWriteTime(directory);
                long timeModification = new DateTimeOffset(datetimeObject).ToUnixTimeSeconds();
                DateTime currentTime = DateTime.Now;
                long signatureCreation = new DateTimeOffset(currentTime).ToUnixTimeSeconds();
                return new Tuple<long, long>(timeModification, signatureCreation);
            }
            finally
            {
            }
        }

        public string OpenHashFile(string directory)
        {
            try
            {
                FileStream openedFile = File.Open(directory, FileMode.Open, FileAccess.Read);
                return HashFile(openedFile);
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

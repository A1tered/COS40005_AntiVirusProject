using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace AsynchronousProgrammingPractice
{
    public class Hasher
    {
        public Hasher()
        {

        }

        public string ConvertHash(string SubjectConversion)
        {
            byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(SubjectConversion);
            SHA256 sha = SHA256.Create();
            byte[] converted = sha.ComputeHash(byteArray);
            for (int b = 0; b < 50000; b++)
            {
                converted = sha.ComputeHash(converted); // Hash 50,000 times.
            }
            return BitConverter.ToString(converted);
        }
    }
}

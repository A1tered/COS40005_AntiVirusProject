using System;
using System.IO;

namespace InstallerScript
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\ProgramData\SimpleAntivirus";

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileHashChecking
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static FileStream OpenFile()
        {
            Console.WriteLine("File Selection\n");
            Console.WriteLine("Please enter the path of the file: ");
            string path = Console.ReadLine();
            try
            {
                FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);
                return file;
            }
            catch (IOException error)
            {
                return null;
            }
        }

        public static string GetHash(FileStream filestream)
        {
            StringBuilder stringBuild = new();
            byte[] file = SHA256.HashData(filestream);

            foreach (byte Byte in file)
            {
                stringBuild.Append(Byte.ToString("X2"));
            }
            filestream.Close();
            return stringBuild.ToString();
        }

        public static void Scan(string hash)
        {
            DirectoryManager dirMan = new DirectoryManager();

            // Create database
            string dbDir = dirMan.getDatabaseDirectory("signatureDB");
            Console.WriteLine($"Database Directory Found: {dbDir}");
            Database signatureDB = new Database(dbDir);
            bool scanResult = signatureDB.Scan(hash);
            if (!scanResult)
            {
                Console.WriteLine("A database error occurred whilst scanning your file. Please try again later.");
            }
        }

        public static void MainMenu()
        {
            Console.WriteLine("Simple File Hash Checker");
            Console.WriteLine("Please choose an option\n");
            Console.WriteLine("1. Get File Hash");
            Console.WriteLine("2. Scan");
            Console.WriteLine("3. Quit");
            Console.WriteLine("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    FileStream path = OpenFile();
                    if (path != null)
                    {
                        // Get hash
                        string hash = GetHash(path);
                        string file = Path.GetFileName(path.Name);
                        Console.WriteLine($"The hash of '{file}' is: {hash}\n");
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid or no file selected.\r\n");
                    }
                    MainMenu();
                    break;
                case "2":
                    FileStream pathToScan = OpenFile();
                    if (pathToScan != null)
                    {
                        string hashToScan = GetHash(pathToScan);
                        Scan(hashToScan);
                        MainMenu();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid or no file selected.\r\n");
                    }
                    break;
                case "3":
                    Console.WriteLine("Program exiting.");
                    break;
                default:
                    Console.WriteLine("No option selected.");
                    MainMenu();
                    break;
            }
        }
    }
}
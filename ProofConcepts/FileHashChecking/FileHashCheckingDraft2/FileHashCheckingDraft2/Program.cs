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
            string dbDir = dirMan.getDatabaseDirectory("SigHashDB");
            Database signatureDB = new Database(dbDir);
            bool scanResult = signatureDB.Scan(hash);
            if (!scanResult)
            {
                Console.WriteLine("Error: A database error occurred whilst scanning your file. Please try again later.");
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
                    Console.Clear();
                    FileStream path = OpenFile();
                    if (path != null)
                    {
                        // Get hash
                        string hash = GetHash(path);
                        string file = Path.GetFileName(path.Name);
                        Console.Clear();
                        Console.WriteLine($"The hash of '{file}' is: {hash}\n");
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Error: Invalid or no file selected.\r\n");
                    }
                    MainMenu();
                    break;
                case "2":
                    Console.Clear();
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
                        Console.Clear();
                        Console.WriteLine("Error: Invalid or no file selected.\r\n");
                    }
                    MainMenu();
                    break;
                case "3":
                    Console.WriteLine("Program exiting.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Error: No option selected.");
                    MainMenu();
                    break;
            }
        }
    }
}
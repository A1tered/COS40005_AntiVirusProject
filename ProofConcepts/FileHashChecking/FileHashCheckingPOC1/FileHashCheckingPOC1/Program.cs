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

        public static string OpenFile()
        {
            Console.WriteLine("File Selection\n");
            Console.WriteLine("Please enter the path of the file: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                return path;
            }
            return null;
        }

        public static string GetHash(, string path)
        {
            StringBuilder stringBuild = new();
            byte[] file = SHA256.HashData(file);

            foreach (byte Byte in file)
            {
                stringBuild.Append(Byte.ToString("x2"));
            }
            return stringBuild.ToString();
        }

        public static void Scan(string hash)
        {

        }

        public static void MainMenu()
        {
            Console.WriteLine("Simple File Hash Checker");
            string path = OpenFile();
            Console.WriteLine("Please choose an option\n");
            Console.WriteLine("1. Get File Hash");
            Console.WriteLine("2. Scan");
            Console.WriteLine("3. Quit");
            Console.WriteLine("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    if (path != null)
                    {
                        // Get hash
                        SHA256 sha256Hash = SHA256.Create();
                        string hash = GetHash(sha256Hash,path);
                        Console.WriteLine($"The hash of '{Path.GetFileName(path)}' is: {hash}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid or no file selected.\r\n");
                    }
                    MainMenu();
                    break;
                case "2":
                    MainMenu();
                    break;
                case "3":
                    Console.WriteLine("Program exiting.");
                    break;
            }
        }
    }
}
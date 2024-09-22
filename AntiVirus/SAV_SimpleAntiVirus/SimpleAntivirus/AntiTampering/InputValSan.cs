/**************************************************************************
* File: InputValSan.cs
* Author: Johann Banaag
* Description: This file is used to sanitise file paths and file names, and validate filenames and file paths. There is a fixed char limit implemented as well. 
* Last Modified: 07/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleAntivirus.AntiTampering
{
    public static class InputValSan
    {
        // the difference between these two patterns is the position of the '^'. 
        // ^[] ensures that it will only accept characters between the [ ]. 
        // [^] is used to find characters that is NOT IN the [ ]. 
        private static readonly string AcceptedChar = @"^[a-zA-Z0-9!#$%'&()\-_@^~+,;=\[\]{}\. ]+$";
        
        public static string GetUserInput()
        {
            Console.WriteLine("Enter a file path:");
            string userInput = Console.ReadLine();

            // validate the input (file path given by the user)
            while (!FilePathValidation(userInput) || !FilePathCharLimit(userInput) || FileNameValidation(userInput) == null)
            {
                if(!FilePathCharLimit(userInput))
                {
                    Console.WriteLine("File path exceeds character limit of 256. Please Try again.");
                }
                else if (!FilePathValidation(userInput))
                {
                    Console.WriteLine("Invalid file path or file path does not exist. Please try again.");
                }
                else if (FileNameValidation(userInput) == null)
                {
                    Console.WriteLine("Invalid naming convention used. Please try again.");
                }
                userInput = Console.ReadLine();
            }

            return userInput;
        }

        public static string FileNameValidation(string path)
        {
            // Maps the actual file name to the FileName variable
            string FileName = Path.GetFileName(path);

            if(!Regex.IsMatch(FileName, AcceptedChar))
            {
                return null;
            }
            return FileName;
        }

        public static string FilePathSanitisation(string path)
        {

            // the user input will be split up into 2 sections, using only the file path/directory
            string DirectoryPath = Path.GetDirectoryName(path);
            string FileName = Path.GetFileName(path);

            // Remove dangerous characters such as '/', '<', etc., null bytes, and control characters
            string SanitisedDirectory = Regex.Replace(DirectoryPath, @"[!@#$%^&{};<>|?*\x00-\x1F]", "");
            string SanitisedFileName = Regex.Replace(FileName, @"[!@#$%^&{};<>|?*\x00-\x1F]", "");


            // replace '..' with "" to reduce the chances of path traversal attacks
            SanitisedDirectory = SanitisedDirectory.Replace("..", "");

            // combine the file path back into 1 string
            string ValSanPath = Path.GetFullPath(Path.Combine(SanitisedDirectory, SanitisedFileName));

            return ValSanPath;
        }

        public static bool FilePathValidation(string path)
        {
            // update > should probably create an input validation that limits the user input to 256 characters (Line 117)
            // validate file path to check if it has null characters or white space and if it is a valid file path format (a drive letter and colon and/or path begins with backslash).
            return !string.IsNullOrWhiteSpace(path) && Path.IsPathRooted(path);
        }

        public static bool FilePathCharLimit(string path)
        {
            // retrieves the number of characters from the user input, if it is bigger than 256 (not including 256) then it will return an error message (line 64).
            return path.Length <= 256;
        }
    }
}

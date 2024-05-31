using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TestAntiVirusInput
{
    static class Program
    {
        static void Main(string[] args)
        {
            ValSanController();
        }

        public static string GetUserInput()
        {
            Console.WriteLine("Enter a file path:");
            string userInput = Console.ReadLine();

            // validate the input (file path given by the user)
            while (!FilePathValidation(userInput) || !FilePathCharLimit(userInput))
            {
                if(!FilePathCharLimit(userInput))
                {
                    Console.WriteLine("File path exceeds character limit of 256. Please Try again.");
                }
                else if (!FilePathValidation(userInput))
                {
                    Console.WriteLine("Invalid file path or file path does not exist. Please try again.");
                }
                userInput = Console.ReadLine();
            }

            return userInput;
        }

        public static string FilePathSanitisation(string path)
        {

            // the user input will be split up into 2 sections, the file path/directory and the actual filename 
            string directoryPath = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            // Remove dangerous characters such as '/', '<', etc., null bytes, and control characters
            string sanitisedDirectory = Regex.Replace(directoryPath, @"[!@#$%^&{};<>:|?*\x00-\x1F]", "");
            string sanitisedFileName = Regex.Replace(fileName, @"[!@#$%^&{};<>:/\\|?*\x00-\x1F]", "");

            // replace '..' with "" to reduce the chances of path traversal attacks
            sanitisedDirectory = sanitisedDirectory.Replace("..", "");
            sanitisedFileName = sanitisedFileName.Replace("..", "");

            // combine the file path back into 1 string
            string sanitizedPath = Path.Combine(sanitisedDirectory, sanitisedFileName);

            return sanitizedPath;
        }

        public static bool FilePathValidation(string path)
        {
            // update > should probably create an input validation that limits the user input to 256 characters (Line 85)
            // validate file path to check if it has null characters or white space and if it is a valid file path format (a drive letter and colon and/or path begins with backslash).
            return !string.IsNullOrWhiteSpace(path) && Path.IsPathRooted(path);
        }

        public static bool FilePathCharLimit(string path)
        {
            // retrieves the number of characters from the user input, if it is bigger than 256 (not including 256) then it will return an error message (line 45).
            return path.Length <= 256;
        }

        public static void ValSanController()
        {
            bool exit = false;
            while (!exit)
            {
                // this string will collect the input of a user and call it into the 'GetUserInput' method which will call into another method that performs input validation
                string userInput = GetUserInput();

                // the userInput from above will be called into the 'FilePathSanitisation' method which performs input sanitisation to which the sanitised path will be initialised to 'sanitisedPath'
                string sanitisedPath = FilePathSanitisation(userInput);

                // Display that the file path has been sanitised
                Console.WriteLine("Sanitized path: " + sanitisedPath);

                // give the user an option to scan another file
                Console.WriteLine("Do you want to scan another file? (Y/N | Y = Yes, N = No)");
                string response = Console.ReadLine();

                // trim ensures that there isn't any leading or trailing whitespace characters in the users input.
                // ToUpper converts the trimmed string into uppercase.
                // != means if user input is not Y, exit will turn to true, breaking/stopping the loop.
                if(exit = response.Trim().ToUpper() != "Y")
                {
                    Console.WriteLine("Exiting program.");
                }
        }
    }
}

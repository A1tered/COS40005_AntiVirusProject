/**************************************************************************
 * File:        [Scanner].cs
 * Author:      [Pawan]
 * Description: Scans all files in a selected directory, computes their hashes,
 *              and outputs the results.
 * Last Modified: [20/08/2024]
 **************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

class Scanner
{
    static async Task Main()
    {
        // Specify the directory you want to scan
        Console.Write("Enter the directory path to scan: ");
        string directoryPath = Console.ReadLine();

        // List to hold file attributes for the final table
        List<FileAttributes> fileAttributesList = new List<FileAttributes>();

        try
        {
            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine($"\nScanning directory: {directoryPath}\n");

                // Recursively scan directories and compute file hashes
                await ScanDirectoryAsync(directoryPath, fileAttributesList);

                // Print the results in a table format
                PrintFileAttributesTable(fileAttributesList);
            }
            else
            {
                Console.WriteLine("The specified directory does not exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    // Scan a directory and all subdirectories
    static async Task ScanDirectoryAsync(string directoryPath, List<FileAttributes> fileAttributesList)
    {
        try
        {
            // Get all files in the directory
            string[] files = Directory.GetFiles(directoryPath);

            // Compute the hash for each file and add to the list
            foreach (string file in files)
            {
                string hash = await ComputeSHA256Async(file);
                FileInfo fileInfo = new FileInfo(file);

                // Add file attributes to the list
                fileAttributesList.Add(new FileAttributes
                {
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                    FileType = fileInfo.Extension,
                    FileSize = fileInfo.Length,
                    Hash = hash
                });
            }

            // Get all subdirectories
            string[] directories = Directory.GetDirectories(directoryPath);

            // Recursively scan each subdirectory
            foreach (string directory in directories)
            {
                await ScanDirectoryAsync(directory, fileAttributesList);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied to directory: {directoryPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while scanning directory: {directoryPath}. Error: {ex.Message}");
        }
    }

    // Compute SHA256 hash for a file
    static async Task<string> ComputeSHA256Async(string filePath)
    {
        try
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                SHA256 sha256 = SHA256.Create();
                byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(stream));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while computing hash for file: {filePath}. Error: {ex.Message}");
            return null;
        }
    }

    // Print file attributes in a nicely formatted table
    static void PrintFileAttributesTable(List<FileAttributes> fileAttributesList)
    {
        // Set column widths for better alignment
        int fileNameWidth = 40;
        int fileTypeWidth = 10;
        int fileSizeWidth = 15;
        int hashWidth = 64;

        // Print table header
        Console.WriteLine($"{"File Name".PadRight(fileNameWidth)} | {"Type".PadRight(fileTypeWidth)} | {"Size (bytes)".PadRight(fileSizeWidth)} | {"Hash".PadRight(hashWidth)}");
        Console.WriteLine(new string('-', fileNameWidth + fileTypeWidth + fileSizeWidth + hashWidth + 9)); // Divider line

        // Print each file's attributes
        foreach (var fileAttr in fileAttributesList)
        {
            Console.WriteLine($"{fileAttr.FileName.PadRight(fileNameWidth)} | {fileAttr.FileType.PadRight(fileTypeWidth)} | {fileAttr.FileSize.ToString().PadRight(fileSizeWidth)} | {fileAttr.Hash.PadRight(hashWidth)}");
        }

        // Print a footer line
        Console.WriteLine(new string('-', fileNameWidth + fileTypeWidth + fileSizeWidth + hashWidth + 9)); // Divider line
    }
}

// Class to hold file attributes
class FileAttributes
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public string Hash { get; set; }
}
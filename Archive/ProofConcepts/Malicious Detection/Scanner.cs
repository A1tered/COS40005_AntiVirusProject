/**************************************************************************
* File:        [Scanner].cs
* Author:      [Pawan]
* Description: [Handles scanning directories and extracting content]
* Last Modified: [17/09/2024]
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace MaliciousCode
{
    public class Scanner
    {
        private readonly DatabaseHandler dbHandler;
        private readonly Detector detector;

        public Scanner(DatabaseHandler dbHandler, Detector detector)
        {
            this.dbHandler = dbHandler;
            this.detector = detector;
        }

        // Scan a directory for supported file types
        public async Task ScanDirectoryAsync(string directoryPath)
        {
            try
            {
                // Recursively scan subdirectories
                string[] directories = Directory.GetDirectories(directoryPath);
                foreach (var directory in directories)
                {
                    await ScanDirectoryAsync(directory);
                }

                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    try
                    {
                        // Create a FileAttributes object to store file metadata
                        FileAttributes fileAttributes = new FileAttributes();
                        FileInfo fileInfo = new FileInfo(file);
                        fileAttributes.FileName = fileInfo.Name;
                        fileAttributes.FileType = fileInfo.Extension;
                        fileAttributes.FileSize = fileInfo.Length;
                        fileAttributes.FileHash = await ComputeSHA1Async(file);
                        fileAttributes.FileContent = await ExtractFileContentAsync(file);
                        // Output file information
                        Console.WriteLine("File Information:");
                        Console.WriteLine($"File Name: {fileAttributes.FileName}");
                        Console.WriteLine($"File Type: {fileAttributes.FileType}");
                        Console.WriteLine($"File Size: {fileAttributes.FileSize} bytes");
                        Console.WriteLine($"File Hash (SHA1): {fileAttributes.FileHash}");
                        // Detect malicious commands in the file content
                        fileAttributes.ContainsMaliciousCommands = detector.ContainsMaliciousCommands(fileAttributes.FileContent);

                        // Output whether the file is malicious or safe
                        Console.WriteLine($"File is {(fileAttributes.ContainsMaliciousCommands ? "Malicious" : "Safe")}");
                        Console.WriteLine("--------------------------------------------------");

                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Access denied to file: {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while scanning file {file}: {ex.Message}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access denied to directory: {directoryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while scanning directory {directoryPath}: {ex.Message}");
            }
        }

        // Compute the SHA1 hash for the file
        private async Task<string> ComputeSHA1Async(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                SHA1 sha1 = SHA1.Create();
                byte[] hashBytes = await Task.Run(() => sha1.ComputeHash(stream));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // Extract file content based on the file type
        private async Task<string> ExtractFileContentAsync(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            // Handle .txt and .bat files
            if (extension == ".txt" || extension == ".bat")
            {
                return await File.ReadAllTextAsync(filePath);
            }

            // Handle .pdf files using iTextSharp
            else if (extension == ".pdf")
            {
                return await Task.Run(() => ExtractTextFromPdf(filePath));
            }

            // If the file is of an unsupported type, return an empty string
            return string.Empty;
        }

        // Extract text from PDFs using iTextSharp (iText5)
        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();

            try
            {
                using (PdfReader reader = new PdfReader(filePath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF {filePath}: {ex.Message}");
            }

            return text.ToString();
        }
    }
}

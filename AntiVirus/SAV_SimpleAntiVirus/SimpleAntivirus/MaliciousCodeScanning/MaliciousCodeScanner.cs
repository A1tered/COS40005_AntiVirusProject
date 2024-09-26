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
using System.Diagnostics;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.FileQuarantine;
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.MaliciousCodeScanning
{
    public class MaliciousCodeScanner
    {
        private readonly DatabaseHandler dbHandler;
        private readonly Detector detector;
        private readonly AlertManager AlertManager;
        private readonly EventBus EventBus;
        public CancellationToken Token;
        private QuarantineManager QuarantineManager;

        public MaliciousCodeScanner(AlertManager alertManager, EventBus eventBus, DatabaseHandler dbHandler, Detector detector, CancellationToken token, QuarantineManager quarantineManager)
        {
            this.dbHandler = dbHandler;
            this.detector = detector;
            AlertManager = alertManager;
            EventBus = eventBus;
            Token = token;
            QuarantineManager = quarantineManager;
        }

        public async Task Scan(string scanType, List<string> customScanDirs)
        {
            await Task.Run(async () =>
            {
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    directories.AddRange
                    ([
                     $"C:\\Program Files",
                     "C:\\Program Files (x86)",
                     "C:\\Windows",
                     System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Startup")
                    ]);
                }
                else if (scanType == "full")
                {
                    string[] drives = Environment.GetLogicalDrives();
                    foreach (string drive in drives)
                    {
                        directories.Add(drive);
                    }
                }
                else if (scanType == "custom")
                {
                    if (customScanDirs != null && customScanDirs.Count > 0)
                    {
                        foreach (string dir in customScanDirs)
                        {
                            Debug.WriteLine($"Currently added dir: {dir}");
                            directories.Add(dir);
                        }
                    }
                }

                foreach (string directorySearch in directories)
                {
                    Token.ThrowIfCancellationRequested();
                    await ScanDirectoryAsync(directorySearch);
                }
            });
        }

        // Scan a directory for supported file types
        public async Task ScanDirectoryAsync(string directoryPath)
        {
            Token.ThrowIfCancellationRequested();
            try
            {
                // Recursively scan subdirectories
                string[] directories = Directory.GetDirectories(directoryPath);
                foreach (var directory in directories)
                {
                    Token.ThrowIfCancellationRequested();
                    await ScanDirectoryAsync(directory);
                }

                string[] files = Directory.GetFiles(directoryPath);

                if (Token.IsCancellationRequested)
                {
                    Token.ThrowIfCancellationRequested();
                }

                try
                {
                    List<string> violationsList = new List<string>();

                    foreach (string file in files)
                    {

                        // Create a FileAttributes object to store file metadata
                        FileAttributes fileAttributes = new FileAttributes();
                        FileInfo fileInfo = new FileInfo(file);
                        fileAttributes.FileName = fileInfo.Name;
                        fileAttributes.FileType = fileInfo.Extension;
                        fileAttributes.FileSize = fileInfo.Length;
                        fileAttributes.FileHash = await ComputeSHA1Async(file);
                        fileAttributes.FileContent = await ExtractFileContentAsync(file);

                        // Debug Output
                        Debug.WriteLine("File Information:");
                        Debug.WriteLine($"File Name: {fileAttributes.FileName}");
                        Debug.WriteLine($"File Type: {fileAttributes.FileType}");
                        Debug.WriteLine($"File Size: {fileAttributes.FileSize} bytes");
                        Debug.WriteLine($"File Hash (SHA1): {fileAttributes.FileHash}");
                        Debug.WriteLine($"File Path: {file}");

                        // Detect malicious commands in the file content
                        fileAttributes.ContainsMaliciousCommands = detector.ContainsMaliciousCommands(fileAttributes.FileContent);
                        Debug.WriteLine($"Contains Malicious Commands: {fileAttributes.ContainsMaliciousCommands}");

                        // Output whether the file is malicious or safe
                        if (fileAttributes.ContainsMaliciousCommands)
                        {
                            violationsList.Add(file);
                        }

                        Debug.WriteLine("--------------------------------------------------");
                    }
                    foreach (string violation in violationsList)
                    {
                        await QuarantineManager.QuarantineFileAsync(violation, EventBus, "maliciouscode");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine($"Access denied to file");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error while scanning file: {ex.Message}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine($"Access denied to directory: {directoryPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while scanning directory {directoryPath}: {ex.Message}");
            }
        }

        // Compute the SHA1 hash for the file
        private async Task<string> ComputeSHA1Async(string filePath)
        {
            Token.ThrowIfCancellationRequested();
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
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
            Token.ThrowIfCancellationRequested();
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
            Token.ThrowIfCancellationRequested();
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
                Debug.WriteLine($"Error extracting text from PDF {filePath}: {ex.Message}");
            }

            return text.ToString();
        }
    }
}

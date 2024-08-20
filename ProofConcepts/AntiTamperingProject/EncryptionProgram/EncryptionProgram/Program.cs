/**************************************************************************
* File:        Program.cs
* Author:      Johann Banaag
* Description: The main functionality of this program is to be able to create folders & files and encrypt it straight away onto the disk/drive
* Last Modified: 20/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace TestEncryptionProgram
{
    class Program
    {
        static void Main()
        {

            // First step = Define file path(s) for folders and create files in those folders
            string[] CreateFolders =
            {
                @"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\TestCreationFolders",
                @"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\EncryptionKey"

            };

            string[] CreateFiles =
            {
                @"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\TestCreationFolders\readme.txt",
                @"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\TestCreationFolders\configfile.txt",
                @"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\TestCreationFolders\userprofile.txt"
            };

            // Second step = If folder already exists, delete the already existing folder(s)
            foreach (string folders in CreateFolders)
            {
                if (Directory.Exists(folders))
                {
                    Directory.Delete(folders, true);
                }
            }

            // Third step = Create a loop/iteration to create each folder and file
            foreach (string folders in CreateFolders)
            {
                try
                {
                    Directory.CreateDirectory(folders);
                    Console.WriteLine($"Created Simple Antivirus folders: {folders}");
                }
                catch (Exception)
                {
                    Console.WriteLine($"There has been an error creating: {folders}");
                }
            }
            foreach (string files in CreateFiles)
            {
                try
                {
                    using FileStream fs = File.Create(files);
                    Console.WriteLine($"Created SAV configuration files: {files}");
                }
                catch (Exception)
                {
                    Console.WriteLine($"There has been an error creating: {files}");
                }
            }

            // Call the GenerateEncryptionKey method
            GenerateEncryptionKey(out byte[] aesKey, out byte[] aesIV);

            // Call the EncryptionKeyStorage method
            EncryptionKeyStorage(aesKey);


        }

        // Creating the method to generate an AES key and IV (initialisation vector)
        private static void GenerateEncryptionKey(out byte[] aeskey, out byte[] aesiv)
        {
            using (Aes aes = Aes.Create())
            {
                // Generate the initialisation vector and assign it to variable iv
                aes.GenerateIV();
                aesiv = aes.IV; 

                // Generate aes key and assign it to variable encryptionkey
                aes.GenerateKey();
                aeskey = aes.Key;

                // Generate a key size of 256
                aes.KeySize = 256; 
            }
        }

        // Creating the method to use DPAPI to encrypt the AES key and store it in a location
        private static void EncryptionKeyStorage(byte[] aesKey)
        {
            // We are using a feature of DPAPI to encrypt our key, this is a Windows OS exclusive
            byte[] DPAPIkey = ProtectedData.Protect(aesKey, null, DataProtectionScope.CurrentUser);

            // We are going to store the ENCRYPTED key in a .dat file in a file path (in the final program, this will be /ProgramData/ directory)
            File.WriteAllBytes(@"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\EncryptionKey\aeskey.dat", DPAPIkey);

        }

        // A method must be created to decrypt the AES key using DPAPI
        private static byte[] RetrieveEncryptionKey()
        {
            byte[] DPAPIkey = File.ReadAllBytes(@"C:\Users\cjban\OneDrive - Swinburne University\2024SEM2\COS40006\SimpleAntiVirus\EncryptionKey\aeskey.dat");
            byte[] normalaeskey = ProtectedData.Unprotect(DPAPIkey, null, DataProtectionScope.CurrentUser);

            return normalaeskey;
        }



    }
}
/**************************************************************************
* File:        Program.cs
* Author:      Johann Banaag
* Description: The main functionality of this program is to be able to encrypt SAV files, decrypt certain functionalities into memory and re-encrypt the files
* Last Modified: 16/09/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.IO;

namespace SimpleAntivirus.AntiTampering
{
    class EncryptionHandler
    {

        public EncryptionHandler()
        {

        }
        //static void Main()
        //{
        //    // Defining the folder/file path to store encryption keys
        //    string[] CreateFolders =
        //    {
        //        @"C:\ProgramData\SimpleAntiVirus\EncryptionKey",
        //        @"C:\ProgramData\SimpleAntiVirus\DatabaseKey"
        //    };

        //    foreach (string folders in CreateFolders)
        //    {
        //        if (Directory.Exists(folders))
        //        {
        //            Directory.Delete(folders, true);
        //        }
        //    }

        //    foreach (string folders in CreateFolders)
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(folders);
        //            Console.WriteLine($"Created Encryption storage: {folders}");
        //        }
        //        catch (Exception errormsg)
        //        {
        //            Console.WriteLine($"Unexpected error creating: {folders}. \nDescription: {errormsg.Message}");
        //        }
        //    }

        //    // Call the GenerateEncryptionKey method 
        //    GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
        //    GenerateDBKey(out byte[] AesKeyDB, out byte[] AesIVDB);

        //    // Call the methods to encrypt the Aes keys and IVs for config files and db
        //    EncryptionKeyStorage(AesKey);
        //    EncryptionIVStorage(AesIV);
        //    EncryptionDBKeyStorage(AesKeyDB);
        //    EncryptionDBIVStorage(AesIVDB);
        //}

        // Creating the method to generate an AES key and IV (initialisation vector)
        // Used the 'out' keyword in the parameters to allow this method to return two pieces of information in a single call without needing to create two seperate methods
        private static void GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV)
        {
            using Aes aes = Aes.Create();

            // Generate the initialisation vector and assign it to variable iv
            aes.GenerateIV();
            AesIV = aes.IV;

            // Generate aes key and assign it to variable encryptionkey
            aes.GenerateKey();
            AesKey = aes.Key;

            // Generate a key size of 256
            aes.KeySize = 256;
        }

        private static void GenerateDBKey(out byte[] AesKeyDB, out byte[] AesIVDB)
        {
            using Aes aes = Aes.Create();

            aes.GenerateIV();
            AesIVDB = aes.IV;

            aes.GenerateKey();
            AesKeyDB = aes.Key;

            aes.KeySize = 256;
        }

        // Creating the method to use DPAPI to encrypt the AES key and store it in a location
        private static void EncryptionKeyStorage(byte[] AesKey)
        {
            // We are using a feature of DPAPI to encrypt our key, this is a Windows OS exclusive
            byte[] DPAPIkey = ProtectedData.Protect(AesKey, null, DataProtectionScope.CurrentUser);

            // We are going to store the ENCRYPTED key in a .dat file in a file path (in the final program, this will be /ProgramData/ directory)
            File.WriteAllBytes(@"C:\ProgramData\SimpleAntiVirus\EncryptionKey\aeskey.dat", DPAPIkey);
        }

        private static void EncryptionIVStorage(byte[] AesIV)
        {
            byte[] DPAPIiv = ProtectedData.Protect(AesIV, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(@"C:\ProgramData\SimpleAntiVirus\EncryptionKey\aesiv.dat", DPAPIiv);
        }

        // Creating a method to use DPAPI to decrypt the .dat file to have access to the AES key, to which the accessible AES key is stored in memory.
        // Use the ClearMemory method to erase the AES key from memory once the DecryptIntoMemory method is complete.
        public static byte[] DecryptionKeyStorage()
        {
            byte[] ReadKey = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirus\EncryptionKey\aeskey.dat");

            byte[] DPAPIkey = ProtectedData.Unprotect(ReadKey, null, DataProtectionScope.CurrentUser);

            return DPAPIkey;
        }

        public static byte[] DecryptionIVStorage()
        {
            byte[] ReadIV = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirus\EncryptionKey\aesiv.dat");

            byte[] DPAPIiv = ProtectedData.Unprotect(ReadIV, null, DataProtectionScope.CurrentUser);

            return DPAPIiv;
        }

        public static void EncryptionDBKeyStorage(byte[] AesKeyDB)
        {
            byte[] DPAPIDBkey = ProtectedData.Protect(AesKeyDB, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(@"C:\ProgramData\SimpleAntiVirus\DatabaseKey\aeskeydb.dat", DPAPIDBkey);
        }

        public static void EncryptionDBIVStorage(byte[] AesIVDB)
        {
            byte[] DPAPIDBiv = ProtectedData.Protect(AesIVDB, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(@"C:\ProgramData\SimpleAntiVirus\DatabaseKey\aesivdb.dat", DPAPIDBiv);
        }

        public static byte[] DecryptionDBKeyStorage()
        {
            byte[] ReadDBKey = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirus\DatabaseKey\aeskeydb.dat");

            byte[] DPAPIDBkey = ProtectedData.Unprotect(ReadDBKey, null, DataProtectionScope.CurrentUser);

            return DPAPIDBkey;
        }

        public static byte[] DecryptionDBIVStorage()
        {
            byte[] ReadDBIV = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirus\DatabaseKey\aesivdb.dat");

            byte[] DPAPIDBiv = ProtectedData.Unprotect(ReadDBIV, null, DataProtectionScope.CurrentUser);

            return DPAPIDBiv;
        }

        // For the 8 functions above, reminder for the other functions calling them that they need to end their call with a
        // CryptographicOperations.ZeroMemory(IV or Key) to erase from memory

        public static void InitialEncryptFiles(string FilePath, byte[] AesKey, byte[] AesIV)
        {
            // This will read the contents of the file
            byte[] FileContents = File.ReadAllBytes(FilePath);
            byte[] EncryptedFileContents;

            // Encrypting process 
            using (Aes aesAlg = Aes.Create())
            {
                // Referencing the parameters in the method
                aesAlg.Key = AesKey;
                aesAlg.IV = AesIV;

                // Create an encryptor to perform this stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(AesKey, AesIV);

                // Create the encryption stream
                using (MemoryStream msEncrypt = new())
                {
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(FileContents);
                    }
                    EncryptedFileContents = msEncrypt.ToArray();
                }
            }

            // The files are now written with a .enc extension
            File.WriteAllBytes(FilePath + ".enc", EncryptedFileContents);

            // We delete the original unencrypted file after encryption to ensure the plaintext files do not remain on disk.
            File.Delete(FilePath);
        }

        public static byte[] DecryptIntoMemory(string FilePath, byte[] AesKey, byte[] AesIV)
        {
            byte[] EncryptedFileContents = File.ReadAllBytes(FilePath);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = AesKey;
                aesAlg.IV = AesIV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(AesKey, AesIV);

                using (MemoryStream msDecrypt = new())
                {
                    using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] DecryptedFileContents = new byte[EncryptedFileContents.Length];
                        csDecrypt.Read(DecryptedFileContents, 0, DecryptedFileContents.Length);
                        return DecryptedFileContents;
                    }
                }
            }
        }

        public static void EncryptFilesAgain(byte[] data, string FilePath, byte[] AesKey, byte[] AesIV)
        {
            byte[] EncryptedFileContents;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = AesKey;
                aesAlg.IV = AesIV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(AesKey, AesIV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                    }
                    EncryptedFileContents = msEncrypt.ToArray();
                }
            }
            File.WriteAllBytes(FilePath, EncryptedFileContents);
        }

        // Call to specifically zero out the memory of the specific addres of a byte using the parameter
        public static void ClearMemory(byte[] data)
        {
            CryptographicOperations.ZeroMemory(data);
        }
    }
}
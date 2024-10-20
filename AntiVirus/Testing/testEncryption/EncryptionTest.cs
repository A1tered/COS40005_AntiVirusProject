/**************************************************************************
* File: EncryptionTest.cs
* Author:      Johann Banaag
* Description: NUnit tests to check if the encryption program works as intended. 
* Last Modified: 1/10/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System.Diagnostics;
using System.Security.Cryptography;
using EncryptionProgram;
using Newtonsoft.Json.Bson;

namespace EncryptionProgramNUnitTest
{
    [TestFixture]
    public class UnitTests
    {
        private string testFilePath;
        private string testfileContent = "Sample text file used for encryption and decryption process.";

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey");
            Directory.CreateDirectory(@"C:\ProgramData\SimpleAntiVirusTest\DatabaseKey");

            string testpath = @"C:\ProgramData\SimpleAntiVirusTest\";

            testFilePath = Path.Combine(testpath, "testfile.txt");
            File.WriteAllText(testFilePath, testfileContent);
        }

        [Test]
        public void GenerateEncryptionKeys()
        {
            // Call the methods to generate encryption keys and IVs for files and db
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
            EncryptionProcess.EncryptionKeyStorage(AesKey);
            EncryptionProcess.EncryptionIVStorage(AesIV);

            EncryptionProcess.GenerateDBKey(out byte[] AesKeyDB, out byte[] AesIVDB);
            EncryptionProcess.EncryptionDBKeyStorage(AesKeyDB);
            EncryptionProcess.EncryptionDBIVStorage(AesIVDB);

            Assert.IsTrue(File.Exists(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey\aeskey.dat"));
            Assert.IsTrue(File.Exists(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey\aesiv.dat"));
            Assert.IsTrue(File.Exists(@"C:\ProgramData\SimpleAntiVirusTest\DatabaseKey\aeskeydb.dat"));
            Assert.IsTrue(File.Exists(@"C:\ProgramData\SimpleAntiVirusTest\DatabaseKey\aesivdb.dat"));

            byte[] ReadAesKey = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey\aeskey.dat");
            byte[] ReadAesIV  = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey\aesiv.dat");
            byte[] ReadAesKeyDB = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirusTest\DatabaseKey\aeskeydb.dat");
            byte[] ReadAesIVDB = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirusTest\DatabaseKey\aesivdb.dat");

            // this will show all the encrypted bytes into a readable format
            Console.WriteLine($"Encrypted Aes Key: {Convert.ToBase64String(ReadAesKey)}");
            Console.WriteLine($"Encrypted IV Key: {Convert.ToBase64String(ReadAesIV)}");
            Console.WriteLine($"Encrypted Aes DB Key: {Convert.ToBase64String(ReadAesKeyDB)}");
            Console.WriteLine($"Encrypted Aes DB IV: {Convert.ToBase64String(ReadAesIVDB)}");
        }

        [Test]
        public void EncryptionKeyIsUnreadable()
        {
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
            EncryptionProcess.EncryptionKeyStorage(AesKey);
            EncryptionProcess.EncryptionIVStorage(AesIV);

            byte[] EncryptedAESKey = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey\aeskey.dat");
            byte[] EncryptedAESIV = File.ReadAllBytes(@"C:\ProgramData\SimpleAntiVirusTest\EncryptionKey\aesiv.dat");

            // checks if the .Protect function encrypts the .dat files to make the generated AES key and IV an unreadable output
            Assert.That(AesKey, Is.Not.EqualTo(EncryptedAESKey));
            Assert.That(AesIV, Is.Not.EqualTo(EncryptedAESIV));

            // below will show the difference in values between the normal and encrypted versions
            Console.WriteLine($"Normal AES Key: {Convert.ToBase64String(AesKey)}");
            Console.WriteLine($"Encrypted AES Key: {Convert.ToBase64String(EncryptedAESKey)}");

            Console.WriteLine($"Normal IV: {Convert.ToBase64String(AesIV)}");
            Console.WriteLine($"Encrypted IV: {Convert.ToBase64String(EncryptedAESIV)}");
        }

        [Test]
        public void EncryptionKeyIsReadable()
        {
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
            EncryptionProcess.EncryptionKeyStorage(AesKey);
            EncryptionProcess.EncryptionIVStorage(AesIV);

            byte[] DecryptedAESKey = EncryptionProcess.DecryptionKeyStorage(AesKey);
            byte[] DecryptedAESIV = EncryptionProcess.DecryptionIVStorage(AesIV);

            // checks if the .Unprotect function decrypts the .dat files to make the generated AES key and IV a readable output
            Assert.That(AesKey, Is.EqualTo(DecryptedAESKey));
            Assert.That(AesIV, Is.EqualTo(DecryptedAESIV));

            // This will prove in a more visual format that both credentials are readable aka the decryption of the .dat files work
            Console.WriteLine($"Normal AES Key: {Convert.ToBase64String(AesKey)}");
            Console.WriteLine($"Decrypted AES Key: {Convert.ToBase64String(DecryptedAESKey)}");

            Console.WriteLine($"Normal IV: {Convert.ToBase64String(AesIV)}");
            Console.WriteLine($"Decrypted IV: {Convert.ToBase64String(DecryptedAESIV)}");
        }

        [Test]
        public void EncryptionOfTestFile()
        {
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
            EncryptionProcess.EncryptionKeyStorage(AesKey);
            EncryptionProcess.EncryptionIVStorage(AesIV);

            EncryptionProcess.DecryptionKeyStorage(AesKey);
            EncryptionProcess.DecryptionIVStorage(AesIV);

            EncryptionProcess.InitialEncryptFiles(testFilePath, AesKey, AesIV);

            string encryptedTestFilePath = testFilePath + ".enc";

            // this checks that the originally defined testFilePath does not exist aka the file "testfile.txt" does not exist
            Assert.IsFalse(File.Exists(testFilePath));

            // this checks that the encrypted version of "testfile.txt" exists which is "testfile.txt.enc".
            Assert.IsTrue(File.Exists(encryptedTestFilePath));

            // write it to the test output to prove that the .enc version exists
            Console.WriteLine(encryptedTestFilePath);

            // this will print out the encrypted contents of testfile.txt.enc
            // NOTE we cannot read the string inside the normal testfile.txt as it does not exist! We will get an error "FileNotFoundException".
            byte[] ReadBytes = File.ReadAllBytes(encryptedTestFilePath);
            Console.WriteLine($"Content of testfile.txt.enc - Encrypted: {Convert.ToBase64String(ReadBytes)}");
        }

        [Test]
        public void DecryptionOfTestFile()
        {
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
            EncryptionProcess.EncryptionKeyStorage(AesKey);
            EncryptionProcess.EncryptionIVStorage(AesIV);

            EncryptionProcess.DecryptionKeyStorage(AesKey);
            EncryptionProcess.DecryptionIVStorage(AesIV);

            EncryptionProcess.InitialEncryptFiles(testFilePath, AesKey, AesIV);

            string encryptedTestFilePath = testFilePath + ".enc";

            // tests that the encrypted file path of the testFilePath exists and the original/normal .txt file does not exist
            Assert.IsFalse(File.Exists(testFilePath));
            Assert.IsTrue(File.Exists(encryptedTestFilePath));

            byte[] decryptedTestFile = EncryptionProcess.DecryptIntoMemory(encryptedTestFilePath, AesKey, AesIV);
            string decryptedIntoString = System.Text.Encoding.UTF8.GetString(decryptedTestFile).TrimEnd('\0');

            // compares that the testFileContent text is equal (and the same) to the text that was decrypted into memory
            // this verifies that the decryption was successful as the string in memory is readable and is the same as testfileContent
            // remember that we cannot get the contents of testfile.txt as that file does not exist after encryption!
            Assert.That(testfileContent, Is.EqualTo(decryptedIntoString));

            Console.WriteLine($"Content of test file - Normal Ver : {testfileContent}");
            Console.WriteLine($"Content of test file - In memory  : {decryptedIntoString}");
        }

        [Test]
        public void Encypt_DecryptedDataFromMemory()
        {
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
            EncryptionProcess.EncryptionKeyStorage(AesKey);
            EncryptionProcess.EncryptionIVStorage(AesIV);

            EncryptionProcess.DecryptionKeyStorage(AesKey);
            EncryptionProcess.DecryptionIVStorage(AesIV);

            EncryptionProcess.InitialEncryptFiles(testFilePath, AesKey, AesIV);

            string encryptedTestFilePath = testFilePath + ".enc";

            byte[] decryptedTestFile = EncryptionProcess.DecryptIntoMemory(encryptedTestFilePath, AesKey, AesIV);
            string decryptedIntoString = System.Text.Encoding.UTF8.GetString(decryptedTestFile).TrimEnd('\0');

            EncryptionProcess.EncryptFilesAgain(decryptedTestFile, encryptedTestFilePath, AesKey, AesIV);

            byte[] decryptedTestFile2 = EncryptionProcess.DecryptIntoMemory(encryptedTestFilePath, AesKey, AesIV);
            string decryptedIntoString2 = System.Text.Encoding.UTF8.GetString(decryptedTestFile2).TrimEnd('\0');

            // Check that the decrypted data matches the decrypted data after re-encryption/calling the EncryptFilesAgain method
            // in other words, compare the decrypted content before and after re-encryption to ensure that the content is the same
            // and that the re-encryption process works
            Assert.That(decryptedIntoString, Is.EqualTo(decryptedIntoString2));

            // this proves in a more visual format that both decrypted strings are same in value before and after the re-encryption process
            Console.WriteLine($"Content of testfile.txt before re-encryption: {decryptedIntoString}");
            Console.WriteLine($"Content of testfile.txt after  re-encryption: {decryptedIntoString2}");


        }
    }

    [TestFixture]
    public class NonFunctionalTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void EncryptionPerformanceTest5MB()
        {
            // Create a test file for certain sizes
            string smallfile = @"C:\ProgramData\SimpleAntiVirusTest\smallfile.txt";

            // fill the file with x amount of j characters.
            File.WriteAllText(smallfile, new string('j', 1024 * 1024 * 5)); // 1024*1024 = 1MB * 5 = 5MB

            // encrypt _____file.txt
            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);

            // Start stopwatch just before encryption
            var Stopwatch1 = new Stopwatch();
            Stopwatch1.Start();
            EncryptionProcess.InitialEncryptFiles(smallfile, AesKey, AesIV);
            // Stop stopwatch as soon as encryption is done
            Stopwatch1.Stop();

            // Output the time it takes to encrypt certain file sizes
            Console.WriteLine($"How long it took to encrypt a 5MB file: {Stopwatch1.ElapsedMilliseconds} ms");
        }

        [Test]
        public void EncryptionPeformanceTest10MB()
        {
            string mediumfile = @"C:\ProgramData\SimpleAntiVirusTest\mediumfile.txt";

            File.WriteAllText(mediumfile, new string('j', 1024 * 1024 * 10));

            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);

            var Stopwatch2 = new Stopwatch();
            Stopwatch2.Start();
            EncryptionProcess.InitialEncryptFiles(mediumfile, AesKey, AesIV);
            Stopwatch2.Stop();

            Console.WriteLine($"How long it took to encrypt a 10MB file: {Stopwatch2.ElapsedMilliseconds} ms");
        }

        [Test]
        public void EncryptionPerformance15MB()
        {
            string bigfile = @"C:\ProgramData\SimpleAntiVirusTest\bigfile.txt";

            File.WriteAllText(bigfile, new string('j', 1024 * 1024 * 15));

            EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);

            var Stopwatch3 = new Stopwatch();
            Stopwatch3.Start();
            EncryptionProcess.InitialEncryptFiles(bigfile, AesKey, AesIV);
            Stopwatch3.Stop();

            Console.WriteLine($"How long it took to encrypt a 15MB file: {Stopwatch3.ElapsedMilliseconds} ms");
        }

        [Test]
        public void EncryptionStressTest()
        {
            // we use parallel.for to leverage multiple cpu cores to simultaneously encrypt several files at once (but not all 100 at once).
            Parallel.For(0, 100, i =>
            {
                string filepath = $"C:\\ProgramData\\SimpleAntiVirusTest\\stresstestfile{i}.txt";
                string filePathContent = "test file for stress testing purposes";
                File.WriteAllText(filepath, filePathContent);
                string encryptedFilePath = filepath + ".enc";

                EncryptionProcess.GenerateEncryptionKey(out byte[] AesKey, out byte[] AesIV);
                EncryptionProcess.InitialEncryptFiles(filepath, AesKey, AesIV);

                Console.WriteLine($"{encryptedFilePath} has been encrypted");
            });
        }
    }
}
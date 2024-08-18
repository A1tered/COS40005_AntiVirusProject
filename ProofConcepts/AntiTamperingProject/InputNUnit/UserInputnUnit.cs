/**************************************************************************
* File: UserInputnUnit.cs
* Author: Johann Banaag
* Description: This file is used to test that any user input in the InputValSan file works and the program acts accordingly to what it is expected to do.
* Last Modified: 08/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using NUnit.Framework;

namespace TestAntiVirusInput.NUnitTests
{
    public class UserInputTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CharLimitTest_Exceed()
        {
            // Creates a file path that exceeds 256 characters
            string exceedinput = @"C:\" + new string('j', 250) + @"\ExampleTestFile.txt";

            // Uses the FilePathCharLimit method to check/test the inputs from above
            bool output1 = InputValSan.FilePathCharLimit(exceedinput);

            // IsFalse to prove if the 'exceedinput' is longer than 256 characters
            Assert.IsFalse(output1);
        }

        [Test]
        public void CharLimitTest_Valid()
        {
            // Creates a file path that does not exceed 256 characters
            string validinput = @"C:\TestDirectory\ExampleTest2File.txt";

            // Uses the FilePathCharLimit method to check/test the inputs from above
            bool output2 = InputValSan.FilePathCharLimit(validinput);

            // IsTrue to prove that 'validinput' is not longer than (or equal to) 256 characters
            Assert.IsTrue(output2);
        }

        [Test]
        public void PathValidationTest_RootedOrNotToBeRooted()
        {
            // create strings that are fixed to a specific drive and one that does not begin with a specific drive or backslash
            string PathNotRooted = @"PathNotRooted\TestFile.txt";
            string PathRooted = @"D:\PathIsRooted\DummyFile.txt";

            // Use FilePathValidation method to check
            bool output_PathNotRooted = InputValSan.FilePathValidation(PathNotRooted);
            bool output_PathRooted = InputValSan.FilePathValidation(PathRooted);

            // IsFalse to prove path is not rooted and IsTrue to prove that the path is rooted
            Assert.IsFalse(output_PathNotRooted);
            Assert.IsTrue(output_PathRooted);
        }

        [Test]
        public void PathValidationTest_NullWhiteSpace()
        {
            // create strings that mimic a null string and a string with white space
            string nullpath = null;
            string whitespace = " ";

            // use FilePathValidation method to check
            bool output_nullpath = InputValSan.FilePathValidation(nullpath);
            bool output_whitespace = InputValSan.FilePathValidation(whitespace);

            // IsFalse to prove that the path is in fact null and/or the path is white space
            Assert.IsFalse(output_nullpath);
            Assert.IsFalse(output_whitespace);
        }

        [Test]
        public void FileNameValidationTest_Valid()
        {
            // Give a file path, FileNameValidation method will extract the FileName of the file
            string FilePath = @"D:\PathIsRooted\DummyFile.txt";
            string FileName = "DummyFile.txt";

            // This will extact the filename from the file path, hence the "string FileName = ..."
            string output_FileName = InputValSan.FileNameValidation(FilePath);

            // FileName is the expected filename given from the file path, and output_FileName is the actual result
            Assert.AreEqual(FileName, output_FileName);
        }

        [Test]
        public void FileNameValidationTest_Invalid()
        {
            // Give a file path, FileNameValidation method will extract the FileName of the file
            string FilePath = @"D:\PathIsRooted\/|*Dummy<>File+.txt";

            // This will extact the filename from the file path, hence the "string FileName = ..."
            string output_FileName = InputValSan.FileNameValidation(FilePath);

            // FileName is the expected filename given from the file path, and output_FileName is the actual result
            Assert.IsNull(output_FileName, null);
        }

        [Test]
        public void FilePathSanitisation_Test()
        {
            // Create a file safe path to see if method works as intended < this is the expected result
            string DirectoryPath = @"D:\PathIsRooted\DummyFile.txt";

            // Use FilePathSanisation method and create the actual result
            string output_FilePath = InputValSan.FilePathSanitisation(DirectoryPath);
            
            // What is "expected" and what the actual result is will be checked to see if they are of equal value
            Assert.AreEqual(@"D:\PathIsRooted\DummyFile.txt", output_FilePath);
        }

        [Test]
        public void FilePathSanitisationTest_DangerousCharacters()
        {
            string DirectoryPath = @"D:\#?PathIsRooted\*DummyFile.txt";

            string output_FilePath = InputValSan.FilePathSanitisation(DirectoryPath);

            Assert.AreEqual(@"D:\PathIsRooted\DummyFile.txt", output_FilePath);
        }

        [Test]
        public void FilePathSanitisationTest_traversalattacks()
        {
            // Create a file safe path to see if method works as intended < this is the expected result
            string DirectoryPath = @"D:\PathIsRooted\..\..\..\etc\password";

            // Use FilePathSanisation method and create the actual result
            string output_FilePath = InputValSan.FilePathSanitisation(DirectoryPath);

            // What is "expected" and what the actual result is will be checked to see if they are of equal value
            Assert.AreEqual(@"D:\PathIsRooted\etc\password", output_FilePath);
        }
    }
}
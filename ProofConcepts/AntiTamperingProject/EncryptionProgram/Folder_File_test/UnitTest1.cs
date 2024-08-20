/**************************************************************************
* File:        UnitTest1.cs
* Author:      Johann Banaag
* Description: The main functionality of this unit test is to test the folder and file creation works as expected and predicted
* Last Modified: 20/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using NUnit.Framework;
using System.IO;
using System;

namespace EncryptionProgram.nUnitTests
{
    [TestFixture]
    public class Tests
    {
        // Took the same code from Program.cs file
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

        [SetUp]
        public void Setup()
        {
            // If folder exists before starting a test, delete folder 
            foreach (string folders in CreateFolders)
            {
                if (Directory.Exists(folders))
                {
                    Directory.Delete(folders, true);
                }
            }
        }

        [Test]
        public void FolderExistence_Test()
        {
            // creating just folders and testing if they exist in the directory given from above
            foreach (string folders in CreateFolders)
            {
                Directory.CreateDirectory(folders);
            }
            foreach (string folders in CreateFolders)
            {
                Assert.IsTrue(Directory.Exists(folders));
            }
        }

        [Test]
        public void FileExistence_Test()
        {
            // have to create folders again, and create files in the folders and test if the files exist IN the folder
            foreach (string folders in CreateFolders)
            {
                Directory.CreateDirectory(folders);
            }

            foreach (string files in CreateFiles)
            {
                using FileStream fs = File.Create(files);
            }

            foreach (string files in CreateFiles)
            {
                Assert.IsTrue(File.Exists(files));
            }

        }
    }
}
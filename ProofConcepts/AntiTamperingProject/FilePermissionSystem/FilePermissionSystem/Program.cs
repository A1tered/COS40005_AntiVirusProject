/**************************************************************************
* File:        Program.cs
* Author:      Johann Banaag
* Description: This program aims to set and define file permissions for regular, administrative and guest users for a host device
* Last Modified: 12/09/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System.Security.AccessControl;

namespace FilePermissionSystem
{
    class Program
    {
        static void Main()
        {
            // This wil be the folder/path that we will define file permissions for
            string AppDirectory = @"C:\ProgramData\SimpleAntiVirus";

            // Collect the current ACL of the file and assign it to a usable variable
            DirectoryInfo savFolder = new DirectoryInfo(AppDirectory);
            DirectorySecurity savACL = savFolder.GetAccessControl();

            // Defining the basic permissions for 3 user groups.
            // Admins are given full control, Users are given at most... the power to execute files and reading permissions.
            // Guests are given one permission which is to read files.

            FileSystemAccessRule AdminACL = new FileSystemAccessRule(
                "Administrators",
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);

            FileSystemAccessRule UsersACL = new FileSystemAccessRule(
                "Users",
                FileSystemRights.ReadAndExecute,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);

            FileSystemAccessRule guestReadACL = new FileSystemAccessRule(
                "Guests",
                FileSystemRights.Read,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);

            // Adds new ACL's to the directory/folder and its subfolders and files
            savACL.ResetAccessRule(AdminACL);
            savACL.ResetAccessRule(UsersACL);
            savACL.ResetAccessRule(guestReadACL);

            // Apply the new file/folder permissions 
            savFolder.SetAccessControl(savACL);

            try
            {
                Console.WriteLine("File permission system has been set for all users.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error: {ex.Message}");
            }
        }
    }
}

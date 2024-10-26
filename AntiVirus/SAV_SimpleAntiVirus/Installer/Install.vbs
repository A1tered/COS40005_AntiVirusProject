' Create the FileSystemObject
Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")

' Define the ProgramData directory path
Dim programDataDirectory
programDataDirectory = "C:\ProgramData\SimpleAntiVirus"

' Check if the ProgramData directory exists and delete it
If fso.FolderExists(programDataDirectory) Then
    fso.DeleteFolder programDataDirectory, True
    MsgBox "ProgramData directory deleted successfully.", vbInformation, "Install"
End If

' Read the installation path from the registry
Dim shell, installPath
Set shell = CreateObject("WScript.Shell")
On Error Resume Next
installPath = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\SimpleAntiVirus\InstallPath")
On Error GoTo 0

' If the installation path is found, clean up the Databases folder but keep the initialisation_databases folder
If installPath <> "" Then
    Dim databasesFolder
    databasesFolder = installPath & "\Databases"
    
    ' Check if the Databases folder exists
    If fso.FolderExists(databasesFolder) Then
        ' Loop through files in the Databases folder
        Dim file
        For Each file In fso.GetFolder(databasesFolder).Files
            fso.DeleteFile file.Path, True
        Next

        ' Loop through subfolders but preserve the initialisation_databases folder
        Dim subfolder
        For Each subfolder In fso.GetFolder(databasesFolder).SubFolders
            If LCase(subfolder.Name) <> "initialisation_databases" Then
                fso.DeleteFolder subfolder.Path, True
            End If
        Next

        MsgBox "Non-initialisation databases deleted, initialisation databases preserved.", vbInformation, "Install"
    End If
End If

' Create the FileSystemObject
Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")

' Define the ProgramData directory path
Dim programDataDirectory
programDataDirectory = "C:\ProgramData\SimpleAntiVirus"

' Check if the ProgramData directory exists and delete it
If fso.FolderExists(programDataDirectory) Then
    fso.DeleteFolder programDataDirectory, True
End If

' Read the installation path from the registry
Dim shell, installPath
Set shell = CreateObject("WScript.Shell")
On Error Resume Next
installPath = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\SimpleAntiVirus\InstallPath")
On Error GoTo 0

' If the installation path is found, delete the Databases folder including initialisation_databases
If installPath <> "" Then
    Dim databasesFolder
    databasesFolder = installPath & "\Databases"
    
    ' Check if the Databases folder exists and delete it completely
    If fso.FolderExists(databasesFolder) Then
        fso.DeleteFolder databasesFolder, True
        MsgBox "All databases including initialisation databases deleted.", vbInformation, "Uninstall"
    End If
End If

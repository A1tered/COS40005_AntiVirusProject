Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")

Dim programDataDirectory
programDataDirectory = "C:\ProgramData\SimpleAntiVirus"

If fso.FolderExists(programDataDirectory) Then
    fso.DeleteFolder programDataDirectory, True
End If

Dim shell, installPath
Set shell = CreateObject("WScript.Shell")
On Error Resume Next
installPath = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\SimpleAntiVirus\InstallPath")
On Error GoTo 0

If installPath <> "" Then
    Dim databasesFolder
    databasesFolder = installPath & "\Databases"
    
    If fso.FolderExists(databasesFolder) Then
        fso.DeleteFolder databasesFolder, True
    End If
End If

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
        Dim file
        For Each file In fso.GetFolder(databasesFolder).Files
            fso.DeleteFile file.Path, True
        Next

        Dim subfolder
        For Each subfolder In fso.GetFolder(databasesFolder).SubFolders
            If LCase(subfolder.Name) <> "initialisation_databases" Then
                fso.DeleteFolder subfolder.Path, True
            End If
        Next
    End If
End If

using System.IO;
using System.Security.AccessControl;
using System.Threading.Tasks;

public class FileMover
{
    public async Task MoveFileToQuarantineAsync(string filePath, string quarantinePath)
    {
        byte[] fileContent = await File.ReadAllBytesAsync(filePath);
        await File.WriteAllBytesAsync(quarantinePath, fileContent);
        File.Delete(filePath);
        SetFilePermissions(quarantinePath);
    }

    public async Task MoveFileFromQuarantineAsync(string quarantinePath, string originalPath)
    {
        byte[] fileContent = await File.ReadAllBytesAsync(quarantinePath);
        await File.WriteAllBytesAsync(originalPath, fileContent);
        File.Delete(quarantinePath);
        RestoreFilePermissions(originalPath);
    }

    private void SetFilePermissions(string filePath)
    {
        // Remove all permissions from the file
        FileInfo fileInfo = new FileInfo(filePath);
        FileSecurity fileSecurity = fileInfo.GetAccessControl();
        fileSecurity.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);

        // Optionally, set specific permissions (e.g., read-only for certain users)
        fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.Read, AccessControlType.Deny));

        fileInfo.SetAccessControl(fileSecurity);
    }

    private void RestoreFilePermissions(string filePath)
    {
        // Restore permissions (for simplicity, allowing full control)
        FileInfo fileInfo = new FileInfo(filePath);
        FileSecurity fileSecurity = fileInfo.GetAccessControl();
        fileSecurity.SetAccessRuleProtection(isProtected: false, preserveInheritance: true);
        fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));

        fileInfo.SetAccessControl(fileSecurity);
    }
}

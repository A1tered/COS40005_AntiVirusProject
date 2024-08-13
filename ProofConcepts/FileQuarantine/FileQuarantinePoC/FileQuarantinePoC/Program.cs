using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    private static QuarantineManager quarantineManager;

    public static async Task Main(string[] args)
    {
        FileHasher fileHasher = new FileHasher();
        FileMover fileMover = new FileMover();
        quarantineManager = new QuarantineManager(fileHasher, fileMover);

        if (args.Length > 0)
        {
            await ExecuteCommand(args);
        }
        else
        {
            await ShowMenu();
        }
    }

    private static async Task ExecuteCommand(string[] args)
    {
        switch (args[0].ToLower())
        {
            case "quarantine":
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: quarantine <filePath> <quarantinePath>");
                }
                else
                {
                    await quarantineManager.QuarantineFileAsync(args[1], args[2]);
                }
                break;
            case "unquarantine":
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: unquarantine <quarantinePath> <restorePath>");
                }
                else
                {
                    await quarantineManager.UnquarantineFileAsync(args[1], args[2]);
                }
                break;
            case "delete":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: delete <quarantinePath>");
                }
                else
                {
                    await quarantineManager.DeleteQuarantinedFileAsync(args[1]);
                }
                break;
            case "whitelist":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: whitelist <filePath>");
                }
                else
                {
                    await quarantineManager.AddToWhitelistAsync(args[1]);
                }
                break;
            case "unwhitelist":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: unwhitelist <filePath>");
                }
                else
                {
                    await quarantineManager.RemoveFromWhitelistAsync(args[1]);
                }
                break;
            default:
                Console.WriteLine("Unknown command.");
                break;
        }
    }

    private static async Task ShowMenu()
    {
        while (true)
        {
            Console.WriteLine("\nFile Quarantine System");
            Console.WriteLine("1. Quarantine File");
            Console.WriteLine("2. Unquarantine File");
            Console.WriteLine("3. Delete Quarantined File");
            Console.WriteLine("4. Add to Whitelist");
            Console.WriteLine("5. Remove from Whitelist");
            Console.WriteLine("6. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await QuarantineFile();
                    break;
                case "2":
                    await UnquarantineFile();
                    break;
                case "3":
                    await DeleteQuarantinedFile();
                    break;
                case "4":
                    await AddToWhitelist();
                    break;
                case "5":
                    await RemoveFromWhitelist();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private static async Task QuarantineFile()
    {
        Console.Write("Enter the file path to quarantine: ");
        string filePath = Console.ReadLine();
        Console.Write("Enter the quarantine path: ");
        string quarantinePath = Console.ReadLine();

        await quarantineManager.QuarantineFileAsync(filePath, quarantinePath);
    }

    private static async Task UnquarantineFile()
    {
        Console.Write("Enter the quarantine path: ");
        string quarantinePath = Console.ReadLine();
        Console.Write("Enter the restore path: ");
        string restorePath = Console.ReadLine();

        await quarantineManager.UnquarantineFileAsync(quarantinePath, restorePath);
    }

    private static async Task DeleteQuarantinedFile()
    {
        Console.Write("Enter the quarantine path: ");
        string quarantinePath = Console.ReadLine();

        await quarantineManager.DeleteQuarantinedFileAsync(quarantinePath);
    }

    private static async Task AddToWhitelist()
    {
        Console.Write("Enter the file path to whitelist: ");
        string filePath = Console.ReadLine();

        await quarantineManager.AddToWhitelistAsync(filePath);
    }

    private static async Task RemoveFromWhitelist()
    {
        Console.Write("Enter the file path to remove from the whitelist: ");
        string filePath = Console.ReadLine();

        await quarantineManager.RemoveFromWhitelistAsync(filePath);
    }
}

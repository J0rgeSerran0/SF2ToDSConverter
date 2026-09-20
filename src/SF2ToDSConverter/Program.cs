using System.Diagnostics;

const string AppName = "SF2ToDSConverter";

Console.WriteLine("==============================================");
Console.WriteLine("             SF2 => DSPRESET");
Console.WriteLine("==============================================");
Console.WriteLine();

string root = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Directory.GetCurrentDirectory();

if (!Directory.Exists(root))
{
    Console.WriteLine($"ERROR: The folder does not exist: {root}");
    return 1;
}

string? converterOverride = args.Length > 1
    ? args[1]
    : null;

string converter = FindConverter(root, converterOverride);

if (converter is null)
{
    Console.WriteLine("ERROR: The converter: 'SF22DS' was not found");
    Console.WriteLine();
    Console.WriteLine("Place SF22DS.exe (Windows) or SF22DS (Linux/macOS)");
    Console.WriteLine("in the root folder, or specify its path as the second argument.");
    return 1;
}

Console.WriteLine($"Root folder  : {root}");
Console.WriteLine($"Converter    : {converter}");
Console.WriteLine();

string[] files;

try
{
    files = Directory
        .EnumerateFiles(root, "*", SearchOption.AllDirectories)
        .Where(f => string.Equals(Path.GetExtension(f), ".sf2", StringComparison.OrdinalIgnoreCase))
        .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
        .ToArray();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR searching for files: {ex.Message}");
    return 1;
}

Console.WriteLine($"SF2 files found: {files.Length}");
Console.WriteLine();

if (files.Length == 0)
{
    Console.WriteLine("There is nothing to convert.");
    return 0;
}

int converted = 0;
int skipped = 0;
int failed = 0;

for (int i = 0; i < files.Length; i++)
{
    string sf2 = files[i];
    string dspreset = Path.ChangeExtension(sf2, ".dspreset");

    Console.WriteLine($"[{i + 1}/{files.Length}] {Path.GetRelativePath(root, sf2)}");

    if (File.Exists(dspreset))
    {
        Console.WriteLine("  SKIPPED: .dspreset already exists");
        skipped++;
        continue;
    }

    try
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = converter,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // ArgumentList avoid problems with spaces and special characters.
        startInfo.ArgumentList.Add(sf2);
        startInfo.ArgumentList.Add(dspreset);

        using Process? process = Process.Start(startInfo);

        if (process is null)
        {
            Console.WriteLine("  ERROR: could not start SF22DS.");
            failed++;
            continue;
        }

        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine("  OK");
            converted++;
        }
        else
        {
            Console.WriteLine($"  ERROR: exit code {process.ExitCode}");

            // If the converter created the file despite the exit code,
            // We leave it intact so that the user can inspect it.
            failed++;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ERROR: {ex.Message}");
        failed++;
    }
}

Console.WriteLine();
Console.WriteLine("==============================================");
Console.WriteLine("                  SUMMARY");
Console.WriteLine("==============================================");
Console.WriteLine($"Found       : {files.Length}");
Console.WriteLine($"Converted   : {converted}");
Console.WriteLine($"Skipped     : {skipped}");
Console.WriteLine($"Errors      : {failed}");
Console.WriteLine();

return failed == 0 ? 0 : 2;

static string? FindConverter(string root, string? overridePath)
{
    if (!string.IsNullOrWhiteSpace(overridePath))
    {
        string path = Path.GetFullPath(overridePath);

        if (File.Exists(path))
            return path;

        Console.WriteLine($"WARNING: The specified converter does not exist: {path}");
    }

    string fileName = OperatingSystem.IsWindows()
        ? "SF22DS.exe"
        : "SF22DS";

    string localPath = Path.Combine(root, fileName);

    return File.Exists(localPath) ? localPath : null;
}

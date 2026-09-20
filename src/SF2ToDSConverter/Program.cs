using System.Diagnostics;

const string AppName = "SF2ToDSConverter";

Console.WriteLine("==============================================");
Console.WriteLine("             SF2 -> DSPRESET");
Console.WriteLine("==============================================");
Console.WriteLine();

string root = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Directory.GetCurrentDirectory();

if (!Directory.Exists(root))
{
    Console.WriteLine($"ERROR: La carpeta no existe: {root}");
    return 1;
}

string? converterOverride = args.Length > 1
    ? args[1]
    : null;

string converter = FindConverter(root, converterOverride);

if (converter is null)
{
    Console.WriteLine("ERROR: No se ha encontrado el conversor SF22DS.");
    Console.WriteLine();
    Console.WriteLine("Coloca SF22DS.exe (Windows) o SF22DS (Linux/macOS)");
    Console.WriteLine("en la carpeta raíz, o indica su ruta como segundo argumento.");
    return 1;
}

Console.WriteLine($"Carpeta raíz : {root}");
Console.WriteLine($"Conversor    : {converter}");
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
    Console.WriteLine($"ERROR al buscar archivos: {ex.Message}");
    return 1;
}

Console.WriteLine($"Ficheros SF2 encontrados: {files.Length}");
Console.WriteLine();

if (files.Length == 0)
{
    Console.WriteLine("No hay nada que convertir.");
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
        Console.WriteLine("  OMITIDO: ya existe el .dspreset");
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

        // ArgumentList evita problemas con espacios y caracteres especiales.
        startInfo.ArgumentList.Add(sf2);
        startInfo.ArgumentList.Add(dspreset);

        using Process? process = Process.Start(startInfo);

        if (process is null)
        {
            Console.WriteLine("  ERROR: no se pudo iniciar SF22DS.");
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
            Console.WriteLine($"  ERROR: código de salida {process.ExitCode}");

            // Si el conversor creó el archivo pese al código de salida,
            // lo dejamos intacto para que el usuario pueda inspeccionarlo.
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
Console.WriteLine("                  RESUMEN");
Console.WriteLine("==============================================");
Console.WriteLine($"Encontrados : {files.Length}");
Console.WriteLine($"Convertidos : {converted}");
Console.WriteLine($"Omitidos    : {skipped}");
Console.WriteLine($"Errores     : {failed}");
Console.WriteLine();

return failed == 0 ? 0 : 2;

static string? FindConverter(string root, string? overridePath)
{
    if (!string.IsNullOrWhiteSpace(overridePath))
    {
        string path = Path.GetFullPath(overridePath);

        if (File.Exists(path))
            return path;

        Console.WriteLine($"AVISO: el conversor indicado no existe: {path}");
    }

    string fileName = OperatingSystem.IsWindows()
        ? "SF22DS.exe"
        : "SF22DS";

    string localPath = Path.Combine(root, fileName);

    return File.Exists(localPath) ? localPath : null;
}

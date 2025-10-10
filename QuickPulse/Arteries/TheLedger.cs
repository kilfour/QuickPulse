using QuickPulse.Instruments;

namespace QuickPulse.Arteries;

public static class TheLedger
{
    public static Ledger Records(string? maybeFileName = null, bool relativeToSolution = true)
        => new(maybeFileName, relativeToSolution);
    public static Ledger Rewrites(string? maybeFileName = null, bool relativeToSolution = true)
        => new Ledger(maybeFileName, relativeToSolution).ClearFile();
}

public class Ledger : IArtery
{
    private readonly string filePath;

    public Ledger(string? maybeFileName = null, bool relativeToSolution = true)
    {
        filePath = GetFilePath(maybeFileName, relativeToSolution);
        EnsureDirectoryExists();
    }

    private static string GetFilePath(string? maybeFileName, bool relativeToSolution)
    {
        var fileName = maybeFileName ?? GetDefaultFileName();
        if (relativeToSolution)
            fileName = RelateToSolution(fileName);
        return Path.GetFullPath(fileName);
    }

    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    private static string GetDefaultFileName()
        => Path.Combine(".quickpulse", $"quick-pulse-{GetUniqueSuffix()}.log");

    private static string GetUniqueSuffix()
        => $"{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}_{Path.GetRandomFileName().Replace(".", "")}";

    private static string RelateToSolution(string fileName)
    {
        var root = SolutionLocator.FindSolutionRoot();
        if (root == null)
            ComputerSays.No("Cannot find solution root.");
        return Path.Combine(root!, fileName);
    }

    public Ledger ClearFile()
        => Chain.It(() => File.WriteAllText(filePath, ""), this);

    public void Absorb(params object[] data)
        => File.AppendAllLines(filePath, data.Select(a => a.ToString()!));
}

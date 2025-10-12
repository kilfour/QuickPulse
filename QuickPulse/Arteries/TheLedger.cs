using QuickPulse.Instruments;

namespace QuickPulse.Arteries;

/// <summary>
/// Factory for creating persistent ledger arteries that record absorbed data to disk.
/// </summary>
public static class TheLedger
{
    /// <summary>
    /// Creates a ledger that appends all absorbed data to a file. Use for long-running traces or audit logs.
    /// </summary>
    public static Ledger Records(string? maybeFileName = null, bool relativeToSolution = true)
        => new(maybeFileName, relativeToSolution);

    /// <summary>
    /// Creates a ledger that clears its file before recording. Use for fresh log runs that should start empty.
    /// </summary>
    public static Ledger Rewrites(string? maybeFileName = null, bool relativeToSolution = true)
        => new Ledger(maybeFileName, relativeToSolution).ClearFile();
}

/// <summary>
/// An artery that writes every absorbed value to a file on disk for persistent tracing and auditing.
/// </summary>
public class Ledger : IArtery
{
    /// <summary>
    /// The full file path where this ledger writes its data.
    /// </summary>
    public string FilePath { get; init; }

    /// <summary>
    /// Creates a new ledger that writes absorbed data to a log file. Resolves relative paths against the solution root when requested.
    /// </summary>
    public Ledger(string? maybeFileName = null, bool relativeToSolution = true)
    {
        FilePath = GetFilePath(maybeFileName, relativeToSolution);
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
        var directory = Path.GetDirectoryName(FilePath);
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

    /// <summary>
    /// Clears the ledger file and returns the same instance. Use to reset logging between runs.
    /// </summary>
    public Ledger ClearFile()
        => Chain.It(() => File.WriteAllText(FilePath, ""), this);

    /// <summary>
    /// Appends all absorbed data as new lines in the ledger file. Use for persistent logging of emitted values.
    /// </summary>
    public void Absorb(params object[] data)
        => File.AppendAllLines(FilePath, data.Select(a => a.ToString()!));
}

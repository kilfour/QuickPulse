using QuickPulse.Instruments;

namespace QuickPulse.Arteries;

/// <summary>
/// Provides factory methods for creating persistent <see cref="FileLogArtery"/> instances
/// that record absorbed flow output to disk.
/// </summary>
public static class FileLog
{
    /// <summary>
    /// Creates a <see cref="FileLogArtery"/> that appends all absorbed data to a file.
    /// Use for continuous traces or audit logs that should retain previous entries.
    /// </summary>
    public static FileLogArtery Append(string? maybeFileName = null, bool relativeToSolution = true)
        => new(maybeFileName, relativeToSolution);

    /// <summary>
    /// Creates a <see cref="FileLogArtery"/> that clears its file before recording.
    /// Use for clean log runs that should start with an empty file.
    /// </summary>
    public static FileLogArtery Write(string? maybeFileName = null, bool relativeToSolution = true)
        => new FileLogArtery(maybeFileName, relativeToSolution).ClearFile();
}

/// <summary>
/// An artery that writes every absorbed value to a file on disk,
/// providing persistent tracing and audit logging for flows.
/// </summary>
public class FileLogArtery : IArtery
{
    /// <summary>
    /// The full file path where this log writes its data.
    /// </summary>
    public string FilePath { get; init; }

    /// <summary>
    /// Creates a new <see cref="FileLogArtery"/> that writes absorbed data to a log file.
    /// Relative paths are resolved against the solution root when <paramref name="relativeToSolution"/> is <see langword="true"/>.
    /// </summary>
    public FileLogArtery(string? maybeFileName = null, bool relativeToSolution = true)
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
    /// Clears the log file and returns the same instance.
    /// Use to reset logging between runs.
    /// </summary>
    public FileLogArtery ClearFile()
        => Chain.It(() => File.WriteAllText(FilePath, ""), this);

    /// <summary>
    /// Appends all absorbed data as new lines to the log file.
    /// Use for durable recording of emitted flow values.
    /// </summary>
    public void Absorb(params object[] data)
        => File.AppendAllLines(FilePath, data.Select(a => a.ToString()!));
}

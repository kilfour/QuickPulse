using QuickPulse.Instruments;

namespace QuickPulse.Arteries.WriteDataBolts;

public class TheFilingCabinet : IAmAFilingCabinet
{
    public char DirectorySeparatorChar { get { return Path.DirectorySeparatorChar; } }
    public string GetFullPath(string path) => Path.GetFullPath(path);
    public string Combine(params string[] paths) => Path.Combine(paths);
    public string? GetDirectoryName(string path) => Path.GetDirectoryName(path);
    public bool DirectoryExists(string directory) => Directory.Exists(directory);
    public void WriteAllText(string path, string? contents) => File.WriteAllText(path, contents);
    public void AppendAllText(string path, string? contents) => File.AppendAllText(path, contents);
    public string? FindSolutionRoot(string? startDirectory = null) => SolutionLocator.FindSolutionRoot();
    public string GetUniqueSuffix() => $"{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}_{Path.GetRandomFileName().Replace(".", "")}";
}

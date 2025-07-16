namespace QuickPulse.Arteries.WriteDataBolts;

public interface IAmAFilingCabinet
{
    char DirectorySeparatorChar { get; }
    string GetFullPath(string path);
    string Combine(params string[] paths);
    string? GetDirectoryName(string path);
    public bool DirectoryExists(string directory);
    void AppendAllText(string path, string? contents);
    void WriteAllText(string path, string? contents);
    public string? FindSolutionRoot(string? startDirectory = null);
}

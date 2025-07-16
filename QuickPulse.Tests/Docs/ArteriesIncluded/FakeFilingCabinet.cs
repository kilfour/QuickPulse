using QuickPulse.Arteries.WriteDataBolts;

namespace QuickPulse.Tests.Docs.BuildFlowTests;

public class FakeFilingCabinet : IAmAFilingCabinet
{
    public char DirectorySeparatorChar => '/';

    public string? SolutionRoot { get; set; } = "/solution";
    public List<(string Path, string? Contents)> Writes = new();
    public List<(string Path, string? Contents)> Appends = new();

    public string GetFullPath(string path) => "/resolved" + path;

    public void WriteAllText(string path, string? contents)
    {
        Writes.Add((path, contents));
    }

    public void AppendAllText(string path, string? contents)
    {
        Appends.Add((path, contents));
    }

    public string? FindSolutionRoot(string? startDirectory = null) => SolutionRoot;

    public string Combine(params string[] paths) => string.Join("/", paths);

    public string? GetDirectoryName(string path)
    {
        return path;
    }

    public bool DirectoryExists(string directory)
    {
        return true;
    }
}

using QuickPulse.Instruments;

namespace QuickPulse.Arteries;

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
}

public class WriteDataToFile : IArtery
{
    private IAmAFilingCabinet filingCabinet;

    private string filePath;

    public WriteDataToFile(string? maybeFileName = null, IAmAFilingCabinet cabinet = null!)
    {
        filingCabinet = cabinet ?? new TheFilingCabinet();
        var fileName = maybeFileName ?? "quick-pulse.log";
        var root = filingCabinet.FindSolutionRoot();
        if (root == null)
            ComputerSays.No("Cannot find solution root.");
        var combined = filingCabinet.Combine(root!, fileName);
        filePath = filingCabinet.GetFullPath(combined);
        var directory = filingCabinet.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !filingCabinet.DirectoryExists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static WriteDataToFile UsingHardCodedPath(string filename, IAmAFilingCabinet cabinet = null!)
    {
        var filingCabinet = cabinet ?? new TheFilingCabinet();
        return new WriteDataToFile() { filingCabinet = filingCabinet, filePath = filingCabinet.GetFullPath(filename) };
    }

    public WriteDataToFile ClearFile()
    {
        filingCabinet.WriteAllText(filePath, "");
        return this;
    }

    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            filingCabinet.AppendAllText(filePath, item.ToString() + Environment.NewLine);
        }
    }
}

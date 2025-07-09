using QuickPulse.Instruments;

namespace QuickPulse.Arteries;

public interface IAmAFilingCabinet
{
    char DirectorySeparatorChar { get; }
    void AppendAllText(string path, string? contents);
    string GetFullPath(string path);
    void WriteAllText(string path, string? contents);
    public string? FindSolutionRoot(string? startDirectory = null);
    string Combine(params string[] paths);
}

public class TheFilingCabinet : IAmAFilingCabinet
{
    public char DirectorySeparatorChar { get { return Path.DirectorySeparatorChar; } }
    public string GetFullPath(string path) => Path.GetFullPath(path);
    public void WriteAllText(string path, string? contents) => File.WriteAllText(path, contents);
    public void AppendAllText(string path, string? contents) => File.AppendAllText(path, contents);
    public string? FindSolutionRoot(string? startDirectory = null) => SolutionLocator.FindSolutionRoot();
    public string Combine(params string[] paths) => Path.Combine(paths);
}

public class WriteDataToFile : IArtery
{
    private IAmAFilingCabinet filingCabinet;

    private string filePath;

    public WriteDataToFile(string? maybeFileName = null, IAmAFilingCabinet cabinet = null!)
    {
        filingCabinet = cabinet ?? new TheFilingCabinet();
        var fileName = maybeFileName ?? "log.txt";
        var root = filingCabinet.FindSolutionRoot();
        if (root == null)
            ComputerSays.No("Cannot find solution root.");
        var combined = filingCabinet.Combine(root!, fileName);
        filePath = filingCabinet.GetFullPath(combined);
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

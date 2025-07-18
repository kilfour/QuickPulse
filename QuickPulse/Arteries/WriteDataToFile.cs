using QuickPulse.Arteries.WriteDataBolts;
using QuickPulse.Instruments;

namespace QuickPulse.Arteries;

public class WriteDataToFile : IArtery
{
    private IAmAFilingCabinet filingCabinet;

    private string filePath;

    public WriteDataToFile(string? maybeFileName = null, IAmAFilingCabinet cabinet = null!)
    {
        filingCabinet = cabinet ?? new TheFilingCabinet();
        string suffix = filingCabinet.GetUniqueSuffix();
        var fileName = maybeFileName ?? filingCabinet.Combine(".quickpulse", $"quick-pulse-{suffix}.log");
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

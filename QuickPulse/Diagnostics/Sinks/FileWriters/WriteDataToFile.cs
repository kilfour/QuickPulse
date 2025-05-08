using QuickPulse.Diagnostics.Instruments;

namespace QuickPulse.Diagnostics.Sinks.FileWriters;

public class WriteDataToFile : IPulse, IPulser
{
    private readonly string filePath;
    private bool hardCodedPath = false;

    public WriteDataToFile(string? maybeFileName = null)
    {
        if (hardCodedPath)
        {
            if (maybeFileName == null)
            {
                ComputerSays.No("You must supply a filename when using a hard coded path.");
            }
            else
            {
                filePath = Path.GetFullPath(maybeFileName);
            }
        }
        var fileName = maybeFileName ?? "/log.txt";
        if (!fileName.StartsWith(Path.DirectorySeparatorChar))
            fileName = Path.DirectorySeparatorChar + fileName;
        var path = SolutionLocator.FindSolutionRoot() + fileName;
        filePath = Path.GetFullPath(path);
    }

    public WriteDataToFile HardCodedPath()
    {
        hardCodedPath = true;
        return this;
    }

    public WriteDataToFile ClearFile()
    {
        File.WriteAllText(filePath, "");
        return this;
    }

    public void Monitor(object data)
    {
        try
        {
            File.AppendAllText(filePath, data.ToString() + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[FileInspector] Failed to log entry: {ex.Message}");
        }
    }

    public void Monitor(params object[] data)
    {
        foreach (var item in data)
        {
            File.AppendAllText(filePath, item.ToString() + Environment.NewLine);
        }
    }
}

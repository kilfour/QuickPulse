using QuickPulse.Diagnostics.Instruments;

namespace QuickPulse.Diagnostics.Sinks.FileWriters;

public class WriteDataToFile : IPulse
{
    private readonly string logFilePath;

    public WriteDataToFile(string? maybePath = null)
    {
        var path = maybePath ?? SolutionLocator.FindSolutionRoot() + "/log.txt";
        logFilePath = Path.GetFullPath(path);
    }
    public WriteDataToFile ClearLog()
    {
        File.WriteAllText(logFilePath, "");
        return this;
    }
    public void Log(object data)
    {
        try
        {
            File.AppendAllText(logFilePath, data.ToString() + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[FileInspector] Failed to log entry: {ex.Message}");
        }
    }
}

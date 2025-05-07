using System.Text.Json;
using System.Text.Json.Serialization;
using QuickPulse.Diagnostics.Instruments;

namespace QuickPulse.Diagnostics.Sinks.FileWriters;

public class WriteToJsonFile : IPulse
{
    private readonly string logFilePath;

    public WriteToJsonFile(string? maybePath = null)
    {
        var path = maybePath ?? SolutionLocator.FindSolutionRoot() + "/log.json";//"/pbt-inspector.ndjson";
        logFilePath = Path.GetFullPath(path);
    }

    public void Monitor(object data)
    {
        try
        {
            string json =
                JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = new LowercaseFirstLetterPolicy(),
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
            File.AppendAllText(logFilePath, json + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[FileInspector] Failed to log entry: {ex.Message}");
        }
    }

    public class LowercaseFirstLetterPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
                return name;

            return char.ToLower(name[0]) + name.Substring(1);
        }
    }
}


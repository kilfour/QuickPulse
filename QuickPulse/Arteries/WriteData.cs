namespace QuickPulse.Arteries;

public static class WriteData
{
    public static WriteDataToFile ToFile(string? maybeFileName = null) => new(maybeFileName);
    public static WriteDataToFile ToNewFile(string? maybeFileName = null) => new WriteDataToFile(maybeFileName).ClearFile();
}

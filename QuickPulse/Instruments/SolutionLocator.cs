namespace QuickPulse.Instruments;

public static class SolutionLocator
{
    public static string? FindSolutionRoot(string? startDirectory = null)
    {
        var dir = new DirectoryInfo(startDirectory ?? Directory.GetCurrentDirectory());
        while (dir != null)
        {
            if (dir.EnumerateFiles().Any(f => f.Extension is ".sln" or ".slnx"))
                return dir.FullName;

            dir = dir.Parent;
        }
        return null;
    }
}
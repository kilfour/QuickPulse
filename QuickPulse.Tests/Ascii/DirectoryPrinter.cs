using QuickPulse.Arteries;

namespace QuickPulse.Tests.Ascii;

public class DirectoryPrinter
{
    [Fact(Skip = "explicit")]
    public void Go()
    {
        var path = @"C:\Code\QuickPulse\QuickPulse.Tests\";
        new WriteDataToFile().ClearFile().Flow(DirectoryToAscii.FromDirectory(path));
    }
}

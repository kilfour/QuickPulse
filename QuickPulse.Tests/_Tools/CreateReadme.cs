using QuickPulse.Explains;

namespace QuickPulse.Tests._Tools;

public class CreateReadme
{
    [Fact]
    public void FromDocAttributes()
    {
        new Document().ToFile("README.md", typeof(CreateReadme).Assembly);
    }
}
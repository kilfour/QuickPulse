using System.Globalization;
using QuickPulse.Bolts;
using QuickPulse.Arteries;
using QuickPulse;
using QuickExplainIt;

namespace QuickPulse.Tests._Tools;

public class CreateReadme
{
    [Fact]
    public void FromDocAttributes()
    {
        new Document().ToFile("README.md", typeof(CreateReadme).Assembly);
    }
}
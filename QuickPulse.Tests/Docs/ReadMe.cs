using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs;

public class ReadMe
{
    [Fact]
    public void GenerateDoc()
    {
        Explain.These<ReadMe>("DocsNew/");
    }
}
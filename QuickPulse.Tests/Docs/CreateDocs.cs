using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs;

public class CreateDocs
{
    [Fact]
    public void Now()
    {
        Explain.These<CreateDocs>("Docs/");
    }
}
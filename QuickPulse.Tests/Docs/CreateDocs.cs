using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs;

public class CreateDocs
{
    [Fact(Skip = "needs package update")]
    public void Now()
    {
        Explain.These<CreateDocs>("Docs/");
    }
}
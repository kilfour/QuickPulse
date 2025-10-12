using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs;

public class CreateDocs
{
    [Fact(Skip = "explicit")]
    public void Now()
    {
        Explain.These<CreateDocs>("Docs/");
        Explain.This<CreateDocs>("Docs/all-in-one.md");
    }
}
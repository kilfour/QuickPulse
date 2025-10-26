using QuickPulse.Explains;
using QuickPulse.Tests.Docs.D_Circulation;

namespace QuickPulse.Tests.Docs;

public class CreateDocs
{
    [Fact]// (Skip = "explicit")
    public void Now()
    {
        // Explain.These<CreateDocs>("Docs/");
        //Explain.This<CreateDocs>("Docs/all-in-one.md");
        Explain.OnlyThis<Circulation>("temp.md");
    }
}
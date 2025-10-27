using QuickPulse.Explains;
using QuickPulse.Tests.Docs.E_CapillariesAndArterioles;

namespace QuickPulse.Tests.Docs;

public class CreateDocs
{
    [Fact(Skip = "explicit")]
    public void Now()
    {
        //Explain.OnlyThis<ReadMe>("README.md");
        Explain.These<CreateDocs>("Docs/");
        Explain.This<CreateDocs>("Docs/all-in-one.md");
    }
}
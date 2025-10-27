using QuickPulse.Explains;
using QuickPulse.Tests.Docs.E_CapillariesAndArterioles.Combinators;

namespace QuickPulse.Tests.Docs.E_CapillariesAndArterioles;

[DocFile]
[DocFileHeader("Capillaries and Arterioles")]
[DocContent(
@"> A.k.a. Pulse Regulation. Branching and conditional control in QuickPulse.
 

So far we've mostly seen flows that travel forever on.
Useful for things like declarative composition,
but where would we be without the ability to branch off an Artery into an Arteriole or even a Capillary.

QuickPulse provides the following ways to control the *direction and branching* of a flow.")]
[DocInclude(typeof(UsingConditionalTernaryOperator))]
[DocInclude(typeof(WhenTests))]
[DocInclude(typeof(TheSomethingIfVariants))]
[DocInclude(typeof(FirstOfTests))]
public class CapillariesAndArterioles
{
}
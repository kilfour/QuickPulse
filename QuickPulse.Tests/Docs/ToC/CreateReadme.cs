using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.ToC;

public class CreateReadme
{
    [Fact]
    [Doc(Order = "0", Caption = "QuickPulse Documentation", Content =
@"
- [A Quick Pulse](AQuickPulse.md)
- [Make it Flow](MakeItFlow.md)
- [One Signal, One State](OneSignalOneState.md)
- [Flow Extensions](FlowExtensions.md)
- [Arteries Included](ArteriesIncluded.md)
- [Examples](Examples.md)
- [Addendum: No Where](NoWhere.md)
")]
    public void FromDocAttributes()
    {
        new Document().ToFile("TempQuickPulseDoc.md", typeof(CreateReadme).Assembly);
        new Document().ToFiles([
            new ("README.md", ["QuickPulse.Tests.Docs.Introduction"]), // Check

            new ("./Docs/ToC.md", ["QuickPulse.Tests.Docs.ToC"]),

            new ("./Docs/AQuickPulse.md", ["QuickPulse.Tests.Docs.AQuickPulse"]), // Check       


            new ("./Docs/MakeItFlow.md", ["QuickPulse.Tests.Docs.MakeItFlow.CheatSheet"]),

            new ("./Docs/MakeItFlowStart.md", ["QuickPulse.Tests.Docs.MakeItFlow.Start"]),
            new ("./Docs/MakeItFlowTrace.md", ["QuickPulse.Tests.Docs.MakeItFlow.Trace"]),
            new ("./Docs/MakeItFlowFirstOf.md", ["QuickPulse.Tests.Docs.MakeItFlow.FirstOf"]),
            new ("./Docs/MakeItFlowEffect.md", ["QuickPulse.Tests.Docs.MakeItFlow.Effect"]),
            new ("./Docs/MakeItFlowGather.md", ["QuickPulse.Tests.Docs.MakeItFlow.Gather"]),
            new ("./Docs/MakeItFlowScoped.md", ["QuickPulse.Tests.Docs.MakeItFlow.Scoped"]),
            new ("./Docs/MakeItFlowToFlow.md", ["QuickPulse.Tests.Docs.MakeItFlow.ToFlow"]),
            new ("./Docs/MakeItFlowWhen.md", ["QuickPulse.Tests.Docs.MakeItFlow.When"]),
            new ("./Docs/MakeItFlowNoOp.md", ["QuickPulse.Tests.Docs.MakeItFlow.NoOp"]),

            new ("./Docs/OneSignalOneState.md", ["QuickPulse.Tests.Docs.OneSignalOneState"]),
            new ("./Docs/FlowExtensions.md", ["QuickPulse.Tests.Docs.FlowExtensions"]),
            new ("./Docs/ArteriesIncluded.md", ["QuickPulse.Tests.Docs.ArteriesIncluded"]), // Check
            new ("./Docs/Examples.md", ["QuickPulse.Tests.Docs.Examples"]),

            new ("./Docs/NoWhere.md", ["QuickPulse.Tests.Docs.NoWhere"]), // Check

            // new ("./Docs/WhyQuickPulseExists.md", ["QuickPulse.Tests.Docs.WhyQuickPulseExists"]),
            
        ], typeof(CreateReadme).Assembly);
    }
}

// public static class DocumentExtensions
// {
//     public static Document ToFilesFromNamespace(
//         this Document doc,
//         string baseFolder,
//         string baseNamespace,
//         Assembly assembly,
//         Func<string, string>? fileNameOverride = null)
//     {
//         var allDocs = DocScanner
//             .From(assembly)
//             .Where(d => d.Namespace?.StartsWith(baseNamespace) == true)
//             .GroupBy(d => d.Namespace!)
//             .OrderBy(g => g.Key);

//         foreach (var group in allDocs)
//         {
//             var nsParts = group.Key.Substring(baseNamespace.Length)
//                 .TrimStart('.')
//                 .Split('.');

//             var fileName = fileNameOverride?.Invoke(group.Key)
//                 ?? (nsParts.Length == 0 || string.IsNullOrWhiteSpace(nsParts[0])
//                     ? "Index.md"
//                     : string.Join("", nsParts) + ".md");

//             var filePath = Path.Combine(baseFolder, fileName);

//             doc.ToFile(filePath, group);
//         }

//         return doc;
//     }
// }
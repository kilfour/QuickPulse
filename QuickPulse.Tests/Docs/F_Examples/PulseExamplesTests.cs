using QuickPulse.Explains.Deprecated;

namespace QuickPulse.Tests.Docs.Examples;

[Doc(Order = Chapters.Examples, Caption = "Some Examples")]
public class PulseExamplesTests
{
    [Doc(Order = Chapters.Examples + "-1", Caption = "Log Filtering", Content =
@"
**Given:**
```csharp
public record DiagnosticInfo(string[] Tags, string Message, int PhaseLevel);
```
We can filter this by tags and indent output based on PhaseLevel like so: 
```csharp
public static Signal<DiagnosticInfo> FilterOnTags(IArtery artery, params string[] filter)
{
    var flow =
        from _ in Pulse.Using(artery)
        from diagnosis in Pulse.Start<DiagnosticInfo>()
        let needsLogging = diagnosis.Tags.Any(a => filter.Contains(a))
        let indent = new string(' ', diagnosis.PhaseLevel * 4)
        from log in Pulse.TraceIf(needsLogging, $""{indent}{diagnosis.Tags.First()}:{diagnosis.Message}"")
        select diagnosis;
    return Signal.From(flow);
}
```
")]
    public void LogFiltering() { /*placeholder*/}

    [Doc(Order = Chapters.Examples + "-2", Caption = "Rendering this Document", Content =
@"
```csharp
public static Flow<DocAttribute> RenderMarkdown =
    from doc in Pulse.Start<DocAttribute>()
    let headingLevel = doc.Order.Split('-').Length
    from rcaption in Pulse
        .NoOp(/* ---------------- Render Caption  ---------------- */ )
    let caption = doc.Caption
    let hasCaption = !string.IsNullOrEmpty(doc.Caption)
    let headingMarker = new string('#', headingLevel)
    let captionLine = $""{headingMarker} {caption}""
    from _t2 in Pulse.TraceIf(hasCaption, () => captionLine)
    from rcontent in Pulse
        .NoOp(/* ---------------- Render content  ---------------- */ )
    let content = doc.Content
    let hasContent = !string.IsNullOrEmpty(content)
    from _t3 in Pulse.TraceIf(hasContent, () => content)
    from end in Pulse
        .NoOp(/* ---------------- End of content  ---------------- */ )
    select doc;
```
")]
    public void RenderingThisDocument() { /*placeholder*/}

}
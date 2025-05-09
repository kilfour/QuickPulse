using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.Docs.ExamplesTests;

[Doc(Order = Chapters.Examples, Caption = "Some Examples")]
public class PulseExamplesTests
{
    [Doc(Order = Chapters.Examples + "-1", Caption = "Log Filtering", Content =
@"
**Given:**
```csharp
public record DiagnosticInfo(string[] Tags, string Message, int PhaseLevel);
```
We can filter this by tags and indent output based on PhaseLevel like so : 
```csharp
public static Signal<DiagnosticInfo> FilterOnTags(IArtery artery, params string[] filter)
{
    var flow =
        from _ in Pulse.Using(artery)
        from diagnosis in Pulse.Start<DiagnosticInfo>()
        let needsLogging = diagnosis.Tags.Any(a => filter.Contains(a))
        let indent = new string(' ', Math.Max(0, diagnosis.PhaseLevel) * 4)
        from log in Pulse.TraceIf(needsLogging, $""{indent}{diagnosis.Tags.First()}:{diagnosis.Message}"")
        select diagnosis;
    return Signal.From(flow); ;
}
```
")]
    public void LogFiltering() { /*placeholder*/}

    [Doc(Order = Chapters.Examples + "-2", Caption = "Rendering this Document", Content =
@"
```csharp
public static Flow<DocAttribute> RenderMarkdown =
    from doc in Pulse.Start<DocAttribute>()
    from previousLevel in Pulse.Gather(0)
    let headingLevel = doc.Order.Split('-').Length
    from first in Pulse.Gather(true)
    from rcaption in Pulse
        .NoOp(/* ---------------- Render Caption  ---------------- */ )
    let caption = doc.Caption
    let hasCaption = !string.IsNullOrEmpty(doc.Caption)
    let headingMarker = new string('#', headingLevel)
    let captionLine = $""{headingMarker} {caption}""
    from _t2 in Pulse.TraceIf(hasCaption, captionLine)
    from rcontent in Pulse
        .NoOp(/* ---------------- Render content  ---------------- */ )
    let content = doc.Content
    let hasContent = !string.IsNullOrEmpty(content)
    from _t3 in Pulse.TraceIf(hasContent, content, """")
    from end in Pulse
        .NoOp(/* ---------------- End of content  ---------------- */ )
    select doc;
```
")]
    public void RenderingThisDocument() { /*placeholder*/}

    [Doc(Order = Chapters.Examples + "-3", Caption = "Transorming Markdown to Json", Content =
@"
```csharp
var json =
    from intAndTextAndBool in Pulse.Start<((int, string), bool)>()
    let intAndText = intAndTextAndBool.Item1
    let escaped = intAndText.Item2.Replace(""\"""", ""\\\"""")
    let comma = !intAndTextAndBool.Item2 ? "", "" : ""[ ""
    from lb in Pulse.Trace($""{comma}{{ \""id\"": {intAndText.Item1}, \""text\"": \""{escaped}\"" }}"")
    select intAndTextAndBool;

var question =
    from line in Pulse.Start<string>()
    from isFirstQuestion in Pulse.Gather(true)
    let trimmed = line.Trim()
    let numberAndTextOrNull = GetLeadingNumberIfFollowedByDot(line)
    let isQuestion = numberAndTextOrNull != null
    from flowed in Pulse.ToFlowIf(isQuestion, json, () => (numberAndTextOrNull.Value, isFirstQuestion.Value))
    from effect in Pulse.EffectIf(isQuestion, () => isFirstQuestion.Value = false)
    select line;

var flow =
    from start in Pulse.Start<string[]>()
    from questions in Pulse.ToFlow(question, start)
    from rb in Pulse.Trace(""]"")
    select start;
```
**Input:**
```
### Rivered 
**When the last card drops ...**

1. **Heb je ooit iets proberen te maken of repareren met een YouTube-tutorial? Wat was het?**  
*Tags: praktisch, zelfredzaamheid*  
*Facilitator note: Goed om zelfstandigheid en digitale leercurves aan te raken.*

2. **Wat is iets dat je hebt gemaakt (digitaal of fysiek) waar je trots op was, ook als het niet werkte?**  
*Tags: creatief, zelfexpressie*  
*Facilitator note: Helpt deelnemers zichzelf als makers te zien.*

3. **Als je het woord ""algoritme"" aan een kind moest uitleggen, wat zou je zeggen?**  
*Tags: technisch, abstract denken*  
*Facilitator note: Laat denkniveau en affiniteit met tech-taal zien.*
```
**Output:**
```json
[ { ""id"": 1, ""text"": ""Heb je ooit iets proberen te maken of repareren met een YouTube-tutorial? Wat was het?"" }
, { ""id"": 2, ""text"": ""Wat is iets dat je hebt gemaakt (digitaal of fysiek) waar je trots op was, ook als het niet werkte?"" }
, { ""id"": 3, ""text"": ""Als je het woord \""algoritme\"" aan een kind moest uitleggen, wat zou je zeggen?"" }
]
```
")]
    public void TransormingMarkdownToJson() { /*placeholder*/}
}




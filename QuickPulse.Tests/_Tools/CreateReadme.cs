using System.Globalization;
using QuickPulse.Bolts;
using QuickPulse.Arteries;
using QuickPulse;

namespace QuickPulse.Tests._Tools;

public class CreateReadme
{
    [Fact]
    public void FromDocAttributes()
    {
        var artery = new WriteDataToFile("README.md").ClearFile();
        var attrs = GetDocAttributes().ToList();
        Signal.From(RenderMarkdown)
            .SetArtery(artery)
            .Pulse(attrs);
    }

    public static Flow<DocAttribute> RenderMarkdown =
        from doc in Pulse.Start<DocAttribute>()
        from previousLevel in Pulse.Gather(0)
        from first in Pulse.Gather(true)
        from rline in Pulse
            .NoOp(/* ---------------- Render Line  ---------------- */ )
        let headingLevel = doc.Order.Split('-').Length
        let needsLine = !first.Value && headingLevel <= previousLevel.Value
        from _t1 in Pulse.TraceIf(needsLine, "---")
        from _a1 in Pulse.Effect(() => { first.Value = false; previousLevel.Value = headingLevel; })
        from rcaption in Pulse
            .NoOp(/* ---------------- Render Caption  ---------------- */ )
        let caption = doc.Caption
        let hasCaption = !string.IsNullOrEmpty(doc.Caption)
        let headingMarker = new string('#', headingLevel)
        let captionLine = $"{headingMarker} {caption}"
        from _t2 in Pulse.TraceIf(hasCaption, captionLine)
        from rcontent in Pulse
            .NoOp(/* ---------------- Render content  ---------------- */ )
        let content = doc.Content
        let hasContent = !string.IsNullOrEmpty(content)
        from _t3 in Pulse.TraceIf(hasContent, content, "")
        select doc;

    private IOrderedEnumerable<DocAttribute> GetDocAttributes()
    {
        var typeattributes =
            typeof(CreateReadme).Assembly
                .GetTypes()
                .SelectMany(t => t.GetCustomAttributes(typeof(DocAttribute), false));

        var methodattributes =
            typeof(CreateReadme).Assembly
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .SelectMany(t => t.GetCustomAttributes(typeof(DocAttribute), false));
        return
            typeattributes
                .Union(methodattributes)
                .Cast<DocAttribute>()
                .OrderBy(attr => ParseOrder(attr.Order), new LexicalFloatArrayComparer());
    }

    public class LexicalFloatArrayComparer : IComparer<float[]>
    {
        public int Compare(float[]? x, float[]? y)
        {
            if (x == null || y == null) return Comparer<float[]>.Default.Compare(x, y);
            var len = Math.Max(x.Length, y.Length);
            for (int i = 0; i < len; i++)
            {
                var a = i < x.Length ? x[i] : 0f;
                var b = i < y.Length ? y[i] : 0f;
                var cmp = a.CompareTo(b);
                if (cmp != 0) return cmp;
            }
            return 0;
        }
    }

    public static float[] ParseOrder(string order)
    {
        return order
            .Split('-')
            .Select(part =>
                float.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var f)
                    ? f
                    : throw new FormatException($"Invalid order segment: '{part}'"))
            .ToArray();
    }
}
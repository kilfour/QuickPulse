using System.Globalization;
using System.Text;

namespace QuickPulse.Tests._Tools;

public class CreateDoc
{
    [Fact]
    public void Go()
    {
        var typeattributes =
            typeof(CreateDoc).Assembly
                .GetTypes()
                .SelectMany(t => t.GetCustomAttributes(typeof(DocAttribute), false));

        var methodattributes =
            typeof(CreateDoc).Assembly
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .SelectMany(t => t.GetCustomAttributes(typeof(DocAttribute), false));

        var additionalAttributes =
            new List<DocAttribute>
            {
                //new() { Order = "1-1", Caption = "QuickAcid Linq 101" }
            };

        var attributes =
            typeattributes
                .Union(methodattributes)
                .Union(additionalAttributes)
                .Cast<DocAttribute>()
                .OrderBy(attr => ParseOrder(attr.Order), new LexicalFloatArrayComparer());

        var sb = new StringBuilder();
        var firstHeader = true;
        var previousHeadingLevel = 0;

        foreach (var attr in attributes)
        {
            if (!string.IsNullOrWhiteSpace(attr.Caption))
            {
                var headingLevel = attr.Order.Split('-').Length;

                if (firstHeader)
                {
                    firstHeader = false;
                }
                else if (headingLevel <= previousHeadingLevel)
                {
                    sb.AppendLine("---");
                    sb.AppendLine();
                }

                previousHeadingLevel = headingLevel;

                var headingMarker = new string('#', headingLevel);
                sb.AppendLine($"{headingMarker} {attr.Caption}");
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(attr.Content))
            {
                sb.AppendLine(attr.Content);
                sb.AppendLine();
            }
        }


        using (var writer = new StreamWriter("../../../../tempdoc.md", false))
            writer.Write(sb.ToString());
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

    private const string Introduction =
@"# QuickMGenerate

## Introduction
An evolution from the QuickGenerate library.

Aiming for : 
- A terser (Linq) syntax.
- A better way of dealing with state.
- Better composability of generators.
- Better documentation.
- Fun.


 ---
";
    private const string AfterThoughts =
@"## After Thoughts

Well ... 
Goals achieved I reckon.
 * **A terser (Linq) syntax** :
For some who are not used it, it might get tricky to get into. 
I must say, I myself, only started using it when I started using [Sprache](https://github.com/sprache/Sprache). 
A beautifull Parsec inspired parsing library.
Stole some ideas from there, I must admit.

 * **A better way of dealing with state, better composability of generators** :
Here's an example of something simple that was quite hard to do in the old QuickGenerate :

```
var generator =
	from firstname in MGen.ChooseFromThese(DataLists.FirstNames)
	from lastname in MGen.ChooseFromThese(DataLists.LastNames)
	from provider in MGen.ChooseFromThese(""yahoo"", ""gmail"", ""mycompany"")
	from domain in MGen.ChooseFromThese(""com"", ""net"", ""biz"")
	let email = string.Format(""{0}.{1}@{2}.{3}"", firstname, lastname, provider, domain)
	select
		new Person
			{
				FirstName = firstname,
				LastName = lastname,
				Email = email
			};
var people = generator.Many(2).Generate();
foreach (var person in people)
{
	Console.Write(person);
}
```
Which outputs something like :
```
  Name : Claudia Coffey, Email : Claudia.Coffey@gmail.net.
  Name : Dale Weber, Email : Dale.Weber@mycompany.biz.
```
 * **Better documentation** : You're looking at it.
 * **Fun** : Well, yes it was.

Even though QuickMGenerate uses a lot of patterns (there's static all over the place) that I usually frown upon,
It's a lot less code, it's a lot more composable, it's, ... well, ... what QuickGenerate should have been.

";
}
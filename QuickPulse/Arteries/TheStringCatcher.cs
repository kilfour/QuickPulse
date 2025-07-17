using System.Text;

namespace QuickPulse.Arteries;

public static class TheString
{
    public static Holden Catcher()
    {
        return new Holden();
    }
}

public class Holden : IArtery
{
    private readonly StringBuilder builder = new();

    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            builder.Append(item?.ToString());
        }
    }

    public void Forgets() => builder.Clear();
    public string Whispers() => builder.ToString();
}
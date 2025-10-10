namespace QuickPulse.Arteries;

public static class TheCollector
{
    public static Collector<T> Exhibits<T>()
    {
        return new Collector<T>();
    }
}

public class Collector<T> : IArtery
{
    public readonly List<T> TheExhibit = [];

    public void Absorb(params object[] data)
    {
        foreach (var item in data)
        {
            TheExhibit.Add((T)item);
        }
    }
}

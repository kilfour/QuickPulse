namespace QuickPulse.Arteries;

public class TheCollector<T> : IArtery
{
    public readonly List<T> Exhibit = [];

    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            Exhibit.Add((T)item);
        }
    }
}

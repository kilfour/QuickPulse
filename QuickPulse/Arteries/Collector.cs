namespace QuickPulse.Arteries;

public class Collector<T> : IArtery
{
    public readonly List<T> Exhibit = [];

    public void Monitor(object data)
    {
        Exhibit.Add((T)data);
    }

    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            Monitor(item);
        }
    }
}

namespace QuickPulse.Diagnostics.Sinks.FileWriters;

public class Collector<T> : IPulse, IPulser
{
    public readonly List<T> Exhibit = [];

    public void Monitor(object data)
    {
        Exhibit.Add((T)data);
    }

    public void Monitor(params object[] data)
    {
        foreach (var item in data)
        {
            Monitor(item);
        }
    }
}

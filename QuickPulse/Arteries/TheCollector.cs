namespace QuickPulse.Arteries;

public class TheCollector<T> : IArtery
{
    public readonly List<T> TheExhibit = [];

    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            TheExhibit.Add((T)item);
        }
    }
}

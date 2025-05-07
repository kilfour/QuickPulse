namespace QuickPulse.Diagnostics.Sinks;

public class DistinctValueInspector<T> : IPulse
{
    public readonly HashSet<T> Seen = new();

    public bool HasSeen(T value)
    {
        return Seen.Contains(value);
    }

    public bool AllTheseHaveBeenSeen(T[] values)
    {
        return values.All(a => Seen.Contains(a));
    }

    public bool HasValueThatSatisfies(Func<T, bool> predicate)
    {
        return Seen.Any(predicate);
    }

    public bool SeenSatisfyEach(Func<T, bool>[] predicates)
    {
        return predicates.All(predicate => Seen.Any(value => predicate(value)));
    }

    public void Monitor(object data)
    {
        if (data is T || data == null)
            Seen.Add((T?)data!);
    }
}
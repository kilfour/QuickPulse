namespace QuickPulse.Arteries;

public static class TheLatch
{
    public static Latch<T> Holds<T>()
    {
        return new Latch<T>();
    }
}

public class Latch<T> : IArtery
{
    private T? value;

    public T? Q => value;

    public void Absorb(params object[] data)
    {
        value = (T)data.Last();
    }
}
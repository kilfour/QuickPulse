using QuickPulse.Arteries;

namespace QuickPulse.Tests.Tools;

public static class TheLatch
{
    public static Latch<T> Holds<T>() => new();
}

public class Latch<T> : IArtery
{
    private T? value;
    public T? Q => value;
    public void Absorb(params object[] data) => value = (T)data.Last();
}

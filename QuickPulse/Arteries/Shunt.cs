namespace QuickPulse.Arteries;

public static class Install
{
    public static readonly IArtery Shunt = new Shunt();
}

public sealed class Shunt : IArtery
{
    public void Absorb(params object[] data) { }
}

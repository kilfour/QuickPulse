namespace QuickPulse.Arteries.Shunt;

public static class Install
{
    private static readonly ShuntArtery shunt = new();
    public static ShuntArtery Shunt { get; } = shunt;
}

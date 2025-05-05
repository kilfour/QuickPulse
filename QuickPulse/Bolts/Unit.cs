namespace QuickPulse.Bolts;

public readonly struct Unit : IComparable<Unit>, IEquatable<Unit>
{
    public static Unit Instance { get; } = new Unit();
    public static bool operator ==(Unit _, Unit __) => true;
    public static bool operator !=(Unit _, Unit __) => false;
    public override bool Equals(object? obj) => obj is Unit;
    public bool Equals(Unit other) => true;
    public int CompareTo(Unit other) => 0;
    public override int GetHashCode() => 0;
}

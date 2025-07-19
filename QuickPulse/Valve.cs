namespace QuickPulse;

public record Valve
{
    private bool open = false;
    private Valve(bool open) { this.open = open; }
    public static Valve Closed() => new(false);
    public static Valve Install() => new(true);
    public void Open() { open = true; }
    public bool Passable() { var result = open; open = false; return result; }
    public bool Restricted() => !Passable();
}
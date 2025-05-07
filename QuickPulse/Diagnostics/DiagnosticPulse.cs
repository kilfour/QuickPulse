namespace QuickPulse.Diagnostics;

public class DiagnosticPulse : IPulse
{
    private readonly Action<object> pulser;
    public DiagnosticPulse(Action<object> pulser) { this.pulser = pulser; }
    public void Log(object data) { pulser(data); }
}

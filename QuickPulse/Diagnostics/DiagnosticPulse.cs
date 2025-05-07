namespace QuickPulse.Diagnostics;

public class DiagnosticPulse : IPulse
{
    private readonly Action<object> pulser;
    public DiagnosticPulse(Action<object> pulser) { this.pulser = pulser; }
    public void Monitor(object data) { pulser(data); }
}

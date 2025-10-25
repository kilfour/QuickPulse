namespace QuickPulse.Tests;

public class SpikeStops
{
    [Fact]
    public void Turns_flow_into_unit_Flow()
    {
        Flow<int> flow = Pulse.Start<int>();
        Assert.IsType<Flow<Flow>>(flow.Dissipate());
    }
}

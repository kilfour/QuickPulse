# Metaphor
Doctors Et All
examination of the symptoms
Anaphylactic shock
If an illness or problem is diagnosed, it is identified.
identify, determine, recognize, distinguish
--- slide --- 
# DOC
where we start
--- slide --- 
Pulse/
├── Core/         → base abstractions
├── Diagnostics/  → logging, shape, test
├── Flow/         → ETL transforms, processors
└── Pulse.csproj  → meta entry

# PulseContext.FromFlow
PulseContext.FromFlow(
    from x in Pulse.From<char[]>()
            from y in Pulse.Shape(() => new string(x))
            from z in Sink.To(() => result = y)
            select x);
Should set PulseContext.Current;
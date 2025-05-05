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

Concept	
Your Code	    Pulse Name Idea
Nurse<T>	    A processing step	            Pulse<T> or Step<T>
State	        Context/env	                    PulseState, Signal, FlowContext
QDiagnosis<T>	Result wrapper	                Reading<T>, PulseResult<T>, Signal<T>
.Track()	    Logging	                        .Trace(), .Observe(), .Echo()
.Shape()	    Transform	                    .Map(), .Morph(), .Transmute()


Diagnose.This   => Open.Book
Nurse<T>        => Bibliophile
State           => Glasses
QDiagnosis<T>   => Draft
Re.Shape        => Re.Vise
Keep.Track()    => Pub.Lish

 pulse = signal = flow
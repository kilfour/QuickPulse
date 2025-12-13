namespace QuickPulse.Arteries;

/// <summary>
/// Defines an output channel for a <see cref="Signal{T}"/>.
/// An <see cref="IArtery"/> receives data emitted by a flow and decides what to do with it,
/// such as recording, displaying, or discarding it.
/// Each call to <see cref="Absorb(object[])"/> represents one pulse traveling through the heart.
/// </summary>
public interface IArtery { public void Absorb(params object[] data); }
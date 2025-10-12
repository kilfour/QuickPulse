using QuickPulse.Instruments;

namespace QuickPulse
{
    /// <summary>
    /// A simple one-shot gate that controls whether a flow can pass through. 
    /// Opens once, then automatically closes after allowing passage.
    /// </summary>
    public record Valve
    {
        private bool open = false;
        private Valve(bool open) { this.open = open; }

        /// <summary>
        /// Creates a valve that starts in the closed position. Use to prevent flow until explicitly opened.
        /// </summary>
        public static Valve Closed() => new(false);

        /// <summary>
        /// Creates a valve that starts open. Use to allow immediate passage until the first check.
        /// </summary>
        public static Valve Install() => new(true);

        /// <summary>
        /// Opens the valve, allowing the next pass to succeed. Use to manually enable a single flow cycle.
        /// </summary>
        public Valve Open() => Chain.It(() => open = true, this);

        /// <summary>
        /// Returns true if the valve was open and closes it afterward. Use to test and consume the open state.
        /// </summary>
        public bool Passable() { var result = open; open = false; return result; }

        /// <summary>
        /// Returns true if the valve is currently closed. Use to check whether passage is blocked.
        /// </summary>
        public bool Restricted() => !Passable();
    }
}

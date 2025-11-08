using System.Text;

namespace QuickPulse.Arteries
{
    /// <summary>
    /// Provides factory methods for creating string-based arteries that capture absorbed output as text.
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Creates a new <see cref="StringSink"/> that collects all absorbed values into a single string.
        /// Use for readable output, logging, or test assertions.
        /// </summary>
        public static StringSink Capture() => new();
    }

    /// <summary>
    /// An artery that captures all absorbed values as a continuous string.
    /// </summary>
    public class StringSink : IArtery
    {
        private readonly StringBuilder builder = new();

        /// <summary>
        /// Absorbs data and appends each item's string representation to the internal buffer.
        /// Use to record or inspect flow output as text.
        /// </summary>
        public void Absorb(params object[] data)
        {
            foreach (var item in data)
                builder.Append(item?.ToString());
        }

        /// <summary>
        /// Clears the internal buffer, discarding all previously captured text.
        /// </summary>
        public StringSink Clear() { builder.Clear(); return this; }

        /// <summary>
        /// Returns the concatenated string of all absorbed values.
        /// </summary>
        public string Content() => builder.ToString();
    }
}

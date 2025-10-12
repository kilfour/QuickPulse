using System.Text;

namespace QuickPulse.Arteries
{
    /// <summary>
    /// Factory for creating string-catching arteries that accumulate absorbed output as text.
    /// </summary>
    public static class TheString
    {
        /// <summary>
        /// Creates a new string catcher that collects all absorbed values into a single string. Use for readable output or test assertions.
        /// </summary>
        public static Holden Catcher() => new();
    }

    /// <summary>
    /// An artery that captures all absorbed values as a continuous string.
    /// </summary>
    public class Holden : IArtery
    {
        private readonly StringBuilder builder = new();

        /// <summary>
        /// Absorbs data and appends its string representation to the internal buffer. Use to record flow output as text.
        /// </summary>
        public void Absorb(params object[] data)
        {
            foreach (var item in data) builder.Append(item?.ToString());
        }

        /// <summary>
        /// Clears the internal buffer, forgetting all previously caught text.
        /// </summary>
        public void Forgets() => builder.Clear();

        /// <summary>
        /// Returns the concatenated string of all absorbed values.
        /// </summary>
        public string Whispers() => builder.ToString();
    }
}

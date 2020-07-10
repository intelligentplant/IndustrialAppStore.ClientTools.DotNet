using System;
using System.Globalization;

using Newtonsoft.Json;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Class describing a general-purpose log message.
    /// </summary>
    public class LogMessage {

        /// <summary>
        /// Gets or sets the UTC message timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; private set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; private set; }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the current UTC time.
        /// </summary>
        public LogMessage() {
            UtcTimestamp = DateTime.UtcNow;
        }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the current UTC time and the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogMessage(string message)
            : this() {
            Message = message;
        }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the specified UTC timestamp and message.
        /// </summary>
        /// <param name="utcTimestamp">The timestamp.</param>
        /// <param name="message">The message.</param>
        [JsonConstructor]
        public LogMessage(DateTime utcTimestamp, string message) : this(message) {
            UtcTimestamp = utcTimestamp;
        }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the specified UTC timestamp, format provider, and formatted message.
        /// </summary>
        /// <param name="utcTimestamp">The timestamp.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The message format template.</param>
        /// <param name="args">The format parameters.</param>
        public LogMessage(DateTime utcTimestamp, IFormatProvider formatProvider, string format, params object[] args) : this(utcTimestamp, String.Format(formatProvider, format, args)) { }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the specified UTC timestamp and formatted message.
        /// </summary>
        /// <param name="utcTimestamp">The timestamp.</param>
        /// <param name="format">The message format template.</param>
        /// <param name="args">The format parameters.</param>
        public LogMessage(DateTime utcTimestamp, string format, params object[] args) : this(utcTimestamp, String.Format(CultureInfo.InvariantCulture, format, args)) { }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the current UTC time, format provider, and formatted message.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The message format template.</param>
        /// <param name="args">The format parameters.</param>
        public LogMessage(IFormatProvider formatProvider, string format, params object[] args) : this(String.Format(formatProvider, format, args)) { }


        /// <summary>
        /// Creates a new <see cref="LogMessage"/> using the current UTC time and formatted message.
        /// </summary>
        /// <param name="format">The message format template.</param>
        /// <param name="args">The format parameters.</param>
        public LogMessage(string format, params object[] args) : this(String.Format(CultureInfo.InvariantCulture, format, args)) { }

    }
}

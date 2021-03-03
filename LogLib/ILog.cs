using System;

namespace LogLib
{
    /// <summary>
    /// Enum that indicates Log Level
    /// </summary>
    public enum LogSeverity
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Action = 3,
        Success = 4,
        Error = 5,
        FatalError = 6,
    }

    /// <summary>
    /// interface to log
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Write Line
        /// </summary>
        /// <param name="severity">severity of the message</param>
        /// <param name="color">console color</param>
        /// <param name="format">format string</param>
        /// <param name="arg">format arguments</param>
        void WriteLine(LogSeverity severity, ConsoleColor color, string format, params object[] arg);
    }
}

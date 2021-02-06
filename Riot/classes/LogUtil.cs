using System;

namespace Riot
{
    /// <summary>
    /// Log utility class
    /// </summary>
    public class LogUtil
    {
        /// <summary>
        /// write an action message to console with blue color
        /// </summary>
        public static void WriteAction(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Action, ConsoleColor.Cyan, format, arg);
        }

        /// <summary>
        /// write a success/pass message to console with green color
        /// </summary>
        public static void WritePassed(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Success ,ConsoleColor.Green, format, arg);
        }

        /// <summary>
        /// write an error message to console with red color
        /// </summary>
        public static void WriteError(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Error, ConsoleColor.Magenta, format, arg);
        }

        /// <summary>
        /// write a Warning message to console with yellow color
        /// </summary>
        public static void WriteWarning(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Warning, ConsoleColor.Yellow, format, arg);
        }

        /// <summary>
        /// write a success/pass message to console with green color
        /// </summary>
        public static void WriteFailed(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Error, ConsoleColor.Red, format, arg);
        }

        /// <summary>
        /// write an info message to console with default color
        /// </summary>
        public static void WriteInfo(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Info, Console.ForegroundColor, format, arg);
        }

        /// <summary>
        /// write a debug message to the console with default color
        /// </summary>
        public static void WriteDebug(string format, params object[] arg)
        {
            WriteLineWithTimeStamp(LogSeverity.Debug, Console.ForegroundColor, "DEBUG " + format, arg);
        }

        /// <summary>
        /// write to console with color
        /// </summary>
        public static void WriteLine(ConsoleColor color, string format, params object[] arg)
        {
            WriteLine(LogSeverity.Info, color, format, arg);
        }

        /// <summary>
        /// write to console with default foreground color
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="arg">Format String Argument</param>
        public static void WriteLine(string format, params object[] arg)
        {
            WriteLine(Console.ForegroundColor, format, arg);
        }

        /// <summary>
        /// set the ILog to do the write. default to IotLog to console.
        /// </summary>
        public static ILog Instance { get; set; } = new IotLog();

        /// <summary>
        /// Log Message with particular log level and prepended time stamp
        /// </summary>
        /// <param name="severity">severity of the message</param>
        /// <param name="color">color of message if on console</param>
        /// <param name="format">format string</param>
        /// <param name="arg">format string args</param>
        internal static void WriteLineWithTimeStamp(LogSeverity severity, ConsoleColor color, string format, params object[] arg)
        {
            DateTime now = DateTime.Now;
            string msgFormat = string.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0} {1}", now, format);
            WriteLine(severity, color, msgFormat, arg);
        }

        /// <summary>
        /// Write Line to ILog
        /// </summary>
        /// <param name="severity">severity of the message</param>
        /// <param name="color">console color</param>
        /// <param name="format">format string</param>
        /// <param name="arg">format arguments</param>
        private static void WriteLine(LogSeverity severity, ConsoleColor color, string format, params object[] arg)
        {
            if (severity >= LogLevel)
            {
                try
                {
                    Instance.WriteLine(severity, color, format, arg);
                }
                catch (Exception err)
                {
                    Console.WriteLine("Error in WriteLine {0}", err.ToString());
                    //Console.WriteLine(format, arg);
                }
            }
        }

        /// <summary>
        /// the LogLevel to control what should be logged
        /// TODO: use bits to control each severity
        /// </summary>
        public static LogSeverity LogLevel { get; set; } = LogSeverity.Debug;
    }
}

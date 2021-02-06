using System;

namespace Riot
{
    /// <summary>
    /// default log to console
    /// </summary>
    public class IotLog : ILog
    {
        /// <summary>
        /// Write Line
        /// </summary>
        /// <param name="severity">severity of the message</param>
        /// <param name="color">console color</param>
        /// <param name="format">format string</param>
        /// <param name="arg">format arguments</param>
        public void WriteLine(LogSeverity severity, ConsoleColor color, string format, params object[] arg)
        {
            lock (Lock)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                try
                {
                    Console.WriteLine(format, arg);
                }
                catch (Exception err)
                {
                    Console.ForegroundColor = originalColor;
                    throw err;
                }
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// used for lock for control color in console output
        /// </summary>
        private static readonly object Lock = new object();
    }
}

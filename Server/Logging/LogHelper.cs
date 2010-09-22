using NLog;
using System;

namespace Server.Logging
{
    /// <summary>
    /// Log levels
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Handles logging messages.
    /// </summary>
    class LogHelper
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Logs a message to the logger
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="level">log level</param>
        /// <param name="sourceException">source exception</param>
        public static void LogMessage(string message, Server.Logging.LogLevel level, Exception sourceException)
        {
            _logger.Log(ConvertToNLogLevel(level), message, sourceException);
        }

        /// <summary>
        /// Logs a message to the logger
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="level">log level</param>
        public static void LogMessage(string message, Server.Logging.LogLevel level)
        {
            _logger.Log(ConvertToNLogLevel(level), message);
        }

        /// <summary>
        /// Converts a generic nServer log level to the underlying log level type.
        /// </summary>
        /// <param name="level">nServer log level</param>
        /// <returns>underlying log level type</returns>
        private static NLog.LogLevel ConvertToNLogLevel(Server.Logging.LogLevel level)
        {
            NLog.LogLevel nlogLevel = NLog.LogLevel.Debug;
            switch (level) {
                case Server.Logging.LogLevel.Debug:
                    nlogLevel = NLog.LogLevel.Debug;
                    break;
                case Server.Logging.LogLevel.Info:
                    nlogLevel = NLog.LogLevel.Info;
                    break;
                case Server.Logging.LogLevel.Warning:
                    nlogLevel = NLog.LogLevel.Warn;
                    break;
                case Server.Logging.LogLevel.Error:
                    nlogLevel = NLog.LogLevel.Error;
                    break;
            }
            return nlogLevel;
        }
    }
}
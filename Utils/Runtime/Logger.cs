using UnityEngine;

namespace antoinegleisberg.Utils
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        Fatal = 6
    }

    public static class Logger
    {
        public static LogLevel MinimumLevel = LogLevel.Trace;

        /// <summary>
        /// The Trace level is for very detailed logs that are typically only useful for diagnosing specific issues or understanding the flow of complex code.
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(object message) => Log(LogLevel.Trace, message, "#808080"); // Gray
        /// <summary>
        /// The Debug level is for general debugging information that can help developers understand the state of the application and identify issues during development.
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(object message) => Log(LogLevel.Debug, message, "#ADD8E6"); // Light Blue
        /// <summary>
        /// The Info level is for informational messages that highlight the progress of the application at a coarse-grained level. These messages are typically used to indicate normal operations or significant events in the application's lifecycle.
        /// </summary>
        /// <param name="message"></param>
        public static void Info(object message) => Log(LogLevel.Info, message, "#FFFFFF");  // White
        /// <summary>
        /// The Warning level is for potentially harmful situations that are not necessarily errors but may require attention. These messages indicate that something unexpected happened or that there might be a problem, but the application can still continue running without immediate issues.
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(object message) => Log(LogLevel.Warning, message, "#FFFF00"); // Yellow
        /// <summary>
        /// The Error level is for error events that might still allow the application to continue running but indicate a significant problem that should be addressed. These messages typically indicate that something went wrong, such as an exception being thrown or a critical operation failing, and they often require immediate attention to prevent further issues.
        /// </summary>
        /// <param name="message"></param>
        public static void Error(object message) => Log(LogLevel.Error, message, "#FF4500"); // Orange-Red
        /// <summary>
        /// The Critical level is for severe error events that indicate a critical failure in the application, often leading to a crash or an unrecoverable state. These messages typically indicate that something went very wrong, such as a major exception being thrown or a critical operation failing, and they require immediate attention to prevent further damage or data loss.
        /// </summary>
        /// <param name="message"></param>
        public static void Critical(object message) => Log(LogLevel.Critical, message, "#FF0000"); // Red
        /// <summary>
        /// The Fatal level is for the most severe error events that indicate a fatal failure in the application, often leading to an immediate crash or shutdown. These messages typically indicate that something went catastrophically wrong, such as a critical exception being thrown or a major operation failing, and they require immediate attention to prevent further damage or data loss. The application is expected to be in an unrecoverable state after a fatal error is logged.
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(object message) => Log(LogLevel.Fatal, message, "#FF0000"); // Red

        private static void Log(LogLevel level, object message, string hexColor)
        {
            // Global Filter
            if (level < MinimumLevel) return;

            string formattedMessage = $"<b>[{level.ToString().ToUpper()}]</b>: <color={hexColor}>{message}</color>";

            // Map to Unity's internal types for console icon filtering
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(formattedMessage);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                case LogLevel.Fatal:
                    UnityEngine.Debug.LogError(formattedMessage);
                    break;
            }
        }
    }
}

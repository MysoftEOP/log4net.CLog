using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace log4net.CLog.Models
{
    public class LogEvent
    {
        public LogEvent()
        {
            Properties = new Dictionary<string, string>();
        }

        public string TimeStamp { get; set; }

        public string Message { get; set; }


        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string LoggerName { get; set; }


        public string Level { get; set; }

        public string MethodName { get; set; }

        public IDictionary<string, string> Properties { get; set; }


        public string ThreadName { get; set; }

        public string HostName { get; set; }

        public static IEnumerable<LogEvent> CreateMany(IEnumerable<LoggingEvent> loggingEvents)
        {
            return loggingEvents.Select(Create).ToArray();
        }

        static LogEvent Create(LoggingEvent loggingEvent)
        {
            var logEvent = new LogEvent
            {
                LoggerName = loggingEvent.LoggerName,
                ThreadName = loggingEvent.ThreadName,
                TimeStamp = loggingEvent.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                ExceptionMessage = loggingEvent.ExceptionObject?.Message,
                StackTrace = loggingEvent.ExceptionObject?.StackTrace,
                Message = loggingEvent.RenderedMessage,
                HostName = Environment.MachineName,
                Level = loggingEvent.Level == null ? null : loggingEvent.Level.DisplayName
            };

            if (loggingEvent.LocationInformation != null)
            {
                logEvent.MethodName = loggingEvent.LocationInformation.MethodName;
            }

           // AddProperties(loggingEvent, logEvent);

            return logEvent;
        }
    }
}

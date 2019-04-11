using log4net.Core;
using System;

namespace log4net.CLog
{
    public class IntervalEvalulator : ITriggeringEventEvaluator
    {
        private DateTime _lastTriggeringEvent = DateTime.MinValue;

        public bool IsTriggeringEvent(LoggingEvent loggingEvent)
        {
            if (_lastTriggeringEvent == DateTime.MinValue)
            {
                _lastTriggeringEvent = loggingEvent.TimeStamp;
                return false;
            }
            else
            {
                TimeSpan diff = loggingEvent.TimeStamp - _lastTriggeringEvent;
                _lastTriggeringEvent = loggingEvent.TimeStamp;
                return (diff.TotalSeconds > IntervalSeconds);
            }
        }

        public int IntervalSeconds { get; set; } = 10;
    }
}

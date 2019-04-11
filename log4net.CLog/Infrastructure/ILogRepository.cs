using log4net.CLog.Models;
using System.Collections.Generic;

namespace log4net.CLog.Infrastructure
{
    public interface ILogRepository
    {
        void Add(IEnumerable<LogEvent> logEvents);
    }
}

using log4net.CLog.Infrastructure;
using System;
using log4net.CLog.Utilities;
using log4net.CLog.LogRepository;
using log4net.CLog.Models;

namespace log4net.CLog
{
    /// <summary>
    /// CLog日志服务追加器
    /// </summary>
    public class CLogAppender: BufferingAppender
    {

        public override string AppenderType => typeof(CLogAppender).Name;

        public string ConnectionString { get; set; }

        protected override void Validate()
        {
            if (ConnectionString == null)
            {
                throw new ArgumentNullException(nameof(ConnectionString));
            }

            if (ConnectionString.Length == 0)
            {
                throw new ArgumentException("连接字符串{0}不能为空".With(nameof(ConnectionString)), nameof(ConnectionString));
            }

            LogstashUri.For(ConnectionString).Validate();

            if (MaxConcurrent == 0)
            {
                throw  new ArgumentOutOfRangeException(nameof(MaxConcurrent),"最大并发数必须大于0");
            }
        }

        protected override ILogRepository CreateLogRepository()
        {
          return  CLogRepository.Create(ConnectionString,MaxConcurrent);
        }
    }
}

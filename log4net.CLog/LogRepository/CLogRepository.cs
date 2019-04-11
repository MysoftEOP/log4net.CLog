using log4net.CLog.Infrastructure;
using log4net.CLog.Models;
using log4net.CLog.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Json;

namespace log4net.CLog.LogRepository
{
    /// <summary>
    /// CLog日志服务仓库
    /// </summary>
    public class CLogRepository : ILogRepository
    {
        private readonly Uri _uri;
        private readonly RestHttpClientHelper _httpClient;

        private int n;
        private CLogRepository(Uri uri, RestHttpClientHelper httpClient)
        {
            _uri = uri; 
            _httpClient = httpClient;
            n = 0;
        }

        public void Add(IEnumerable<LogEvent> logEvents)
        {
            var listEvents = logEvents.ToList();
            if (listEvents == null || !listEvents.Any()) { return; }

            
            SaveToCLogTask(listEvents);
        }

        private void SaveToCLogTask(List<LogEvent> logEvents)
        {
            if (logEvents == null) throw new ArgumentNullException(nameof(logEvents));
            if (!logEvents.Any()) return;
            foreach (var logEvent in logEvents)
            {
                try
                {
                    var unused = _httpClient.Post(_uri, ToClogBody( logEvent)).Result;
                    if (log4net.Util.LogLog.QuietMode == false)
                    {
                        if (n<Int32.MaxValue)
                        {
                            Interlocked.Increment(ref n);
                        }
                        else
                        {
                            n = 0;
                        }
                        Console.WriteLine(
                            $"threader id :{Thread.CurrentThread.ManagedThreadId} response: {unused.Content}  {n}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"写入日志【{logEvent.Message}】到日志服务失败:{ex.Message}");
                }
            }
        }
        private string ToClogBody(LogEvent logEvent)
        {
            string body = string.Empty;
            JsonValue messageObject;
            try
            {
                //json格式的message
                messageObject = JsonObject.Parse(logEvent.Message);
            }
            catch
            {
                //字符串message
                messageObject = new JsonObject();
                messageObject["Message"] = logEvent.Message;
            }
            if (messageObject.ContainsKey("Level") == false)
            {
                messageObject["Level"] = logEvent.Level;
            }
            if (messageObject.ContainsKey("ExceptionMessage") == false)
            {
                messageObject["ExceptionMessage"] = logEvent.ExceptionMessage;
            }
            if (messageObject.ContainsKey("logTime") == false)
            {
                messageObject["LogTime"] = logEvent.TimeStamp;
            }
            body = messageObject.ToString();
            return body;

        }

        public static ILogRepository Create(string connectionString, int maxConcurrent)
        {
            return new CLogRepository(LogstashUri.For(connectionString),new RestHttpClientHelper());
        }

    }
}

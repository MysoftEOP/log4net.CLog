using System;
using System.Collections.Specialized;
using log4net.CLog.Utilities;

namespace log4net.CLog.Models
{
    public class LogstashUri
    {
        private readonly StringDictionary _connectionStringParts;

        LogstashUri(StringDictionary connectionStringParts)
        {
            _connectionStringParts = connectionStringParts;
        }

        public static implicit operator Uri(LogstashUri uri)
        {
            //API接口地址: https://{service}/{app}/{identity}/{logtype}
            return new Uri(string.Format("{0}/{1}/{2}/{3}", uri.Server(), uri.App(), uri.Identity(), uri.LogType()));
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Server())||string.IsNullOrWhiteSpace(App())||string.IsNullOrEmpty(LogType()))
            {
                throw new FormatException("连接字符串格式不正确");
            }
        }

        public static LogstashUri For(string connectionString)
        {
            return new LogstashUri(connectionString.ConnectionStringParts());
        }

        string Server()
        {
            return _connectionStringParts[Keys.Server];
        }

        string App()
        {
            return _connectionStringParts[Keys.App];
        }

        string Identity()
        {
            return _connectionStringParts[Keys.Identity];
        }

        string LogType()
        {
            return _connectionStringParts[Keys.LogType];
        }



        private static class Keys
        {
            public const string App = "app";
            public const string Identity = "identity";
            public const string LogType = "logtype";
            public const string Server = "server";
        }
    }
}

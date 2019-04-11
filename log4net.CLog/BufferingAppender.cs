using log4net.Appender;
using log4net.Core;
using log4net.CLog.Infrastructure;
using log4net.CLog.Models;
using log4net.CLog.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace log4net.CLog
{
    public abstract class  BufferingAppender : BufferingAppenderSkeleton
    {
        public const int DefaultOnCloseTimeout = 30000;
        private readonly ManualResetEvent _workQueueEmptyEvent;
        private  Timer _timer;
        private  DateTime _lastSenderBufferDate = DateTime.MinValue;

        int _queuedCallbackCount;
        ILogRepository _logRepository;

        protected BufferingAppender()
        {
            _workQueueEmptyEvent = new ManualResetEvent(true);
            OnCloseTimeout = DefaultOnCloseTimeout;

            SetDefaultMaxConcurrent();
        }


        public int OnCloseTimeout { get; set; }

        /// <summary>
        /// 自动提交时间间隔 单位:秒
        /// </summary>
        public int AutoFlushInterval { get; set; } = 5;

        /// <summary>
        /// 最大并发数
        /// </summary>
        public int MaxConcurrent { get; set; } = 10;

        public virtual string AppenderType => typeof(CLogAppender).Name;

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            SetEnableAutoFlush();

            Validate();


            _logRepository = CreateLogRepository();
        }

        /// <summary>
        /// 设置默认最大CLog服务请求 并发线程数
        /// 默认值取cpu核心数*1.5
        /// </summary>
        private void SetDefaultMaxConcurrent()
        {
            ThreadPool.GetMinThreads(out int workerThreads, out int ioThreads);
            MaxConcurrent = (int)(ioThreads * 1.5);
        }


        private void SetEnableAutoFlush()
        {
            if (BufferSize > 1)
            {
                if (AutoFlushInterval <= 0)
                {

                    throw new ArgumentException("自动提交时间间隔需要大于0", nameof(AutoFlushInterval));
                }

                if (_timer == null)
                {
                    _timer = new Timer((last) =>
                    {
                        if (DateTime.Now.AddSeconds(-AutoFlushInterval) > _lastSenderBufferDate)
                        {
                            Flush(true);
                        }
                    }, _lastSenderBufferDate, AutoFlushInterval * 1000, AutoFlushInterval * 1000);
                }
            }

        }

        private void DisposeTimer()
        {
            if (_timer == null) return;
            _timer.Change(-1, -1);
            _timer.Dispose();
            _timer = null;
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {

            BeginAsyncSend();
            if (TryAsyncSend(events)) return;
            EndAsyncSend();
            HandleError("调用TryAsyncSend方法失败");
        }

        protected override void OnClose()
        {
            base.OnClose();

            DisposeTimer();

            if (TryWaitAsyncSendFinish()) return;
            HandleError("OnClose调用TryWaitAsyncSendFinish方法失败");
        }

        protected abstract ILogRepository CreateLogRepository();


        protected abstract void Validate();

        protected virtual bool TryAsyncSend(IEnumerable<LoggingEvent> events)
        {
            return ThreadPool.QueueUserWorkItem(SendBufferCallback, LogEvent.CreateMany(events));
        }

        protected virtual bool TryWaitAsyncSendFinish()
        {
            return _workQueueEmptyEvent.WaitOne(OnCloseTimeout, false);
        }

        private void BeginAsyncSend()
        {
            _workQueueEmptyEvent.WaitOne();

            if (_queuedCallbackCount == MaxConcurrent-1)
            {
                _workQueueEmptyEvent.Reset();
            }
            Interlocked.Increment(ref _queuedCallbackCount);


        }

        private void SendBufferCallback(object state)
        {
            try
            {
                _logRepository.Add((IEnumerable<LogEvent>)state);
            }
            catch (Exception ex)
            {
                HandleError("未能添加logEvents到{0}，回调SendBufferCallback时报错".With(_logRepository.GetType().Name), ex);
            }
            finally
            {
                EndAsyncSend();
            }
        }

        private void EndAsyncSend()
        {
            if (Interlocked.Decrement(ref _queuedCallbackCount) > 0)
                return;
            _workQueueEmptyEvent.Set();

            _lastSenderBufferDate = DateTime.Now;

        }

        private void HandleError(string message)
        {
            ErrorHandler.Error("{0} [{1}]: {2}.".With(AppenderType, Name, message));
        }

        /// <summary>
        /// 默认处理类
        /// https://github.com/apache/logging-log4net/blob/master/src/Util/LogLog.cs
        /// 默认实现：OnlyOnceErrorHandler 标准信息输出
        /// <appSettings>
        ///  <add key = "log4net.Internal.Debug" value="true" />
        ///  <add key = "log4net.Internal.Quiet" value="false" />
        /// </appSettings>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        private void HandleError(string message, Exception ex) => ErrorHandler.Error("{0} [{1}]: {2}.".With(AppenderType, Name, message), ex, ErrorCode.GenericFailure);

    }
}

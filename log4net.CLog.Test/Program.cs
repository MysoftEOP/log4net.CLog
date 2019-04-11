using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using log4net.Config;
using Newtonsoft.Json;

namespace log4net.CLog.Test
{

    class Program
    {
        static void Main(string[] args)
        {
            InitLog4Net();
            //创建日志记录组件实例
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
          
            TestLog(log);
            //PerformanceTest(log);

            //log.Logger.Repository.Shutdown();


            Console.WriteLine("执行完毕");
            Console.ReadLine();


        }
        private void TestJsonMessage(ILog log)
        {
            CiPerformanceInfo ciPerformanceInfo = new CiPerformanceInfo()
            {
                Application = "platform",
                DeploymentId = 12,
                LogTime = new DateTime()
            };
            var paramData = JsonConvert.SerializeObject(ciPerformanceInfo);
            log.Info(paramData);
        }


        private static void PerformanceTest(ILog log)
        {
            string tag = Guid.NewGuid().ToString();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                Task task = new Task(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        string message = string.Format(@"测试编号：{1} 写入第{0}条日志", j, tag);
                        log.Info(message);
                    }
                }
                    );
                task.Start();
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static void TestLog(ILog log)
        {
            //记录错误日志
            log.Error("error", new Exception("发生了一个异常"));
            //记录严重错误
            log.Fatal("fatal", new Exception("发生了一个致命错误"));
            //记录一般信息
            log.Info("info");
            //记录调试信息
            log.Debug("debug");
        }

        public static void InitLog4Net()
        {
            //配置文件
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/log4net.config");
            //加载配置
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }
    }
}

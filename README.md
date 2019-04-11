# 介绍
log4net.CLog是开放平台开源的log4net的日志输出插件, 用于输出日志消息到明源云的日志平台CLog。对于使用log4net来记录日志的应用，log4net.CLog简单易用，只需修改log4net配置文件，无需任何代码修改即可将日志输出到明源云的日志平台。

log4net.CLog组件已发布到nuget gallery, 地址:https://www.nuget.org/packages/log4net.CLog

# 快速使用

1. 安装插件
</br>Install-Package log4net.CLog -Version 1.0.3

2. 修改log4net配置文件
</br>**配置说明**
</br>appender.connectionString.server - 指定CLog云平台的日志接口地址 
</br>appender.connectionString.app - 当前的应用名称， 例如rdc
</br>appender.connectionString.identity - 日志的标识，可以试环境标识（例如prod/test.dev），也可以是其他标识（例如ERP以customerId作为标识, 由各个应用自行定义， 无限制
</br>appender.connectionString.logtype - 日志类型，例如，行为日志(behavior), 性能日志(performance), 异常日志(exception), 由各个应用自行定义, 无限制
</br>appender.bufferSize - 日志缓存区大小（日志条数），如果是1，那么所有的日志都是即时提交到日志云平台，如果是其他值（例如10），当日志数量达到10时，会将10条日志批量提交到日志云平台
</br>appender.autoFlushInterval - 自动提交缓存区日志的间隔时间，单位是秒。例如值是5，那么每隔5秒，也会自动的将缓存区中未提交的日志提交到日志云平台

```
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
    <appender name="CLogAppender" type="log4net.CLog.CLogAppender, log4net.CLog">
        <connectionString value="server= https://{logapihost};app={app};identity={env};logtype={logtype}"/>       
        <bufferSize value="1" />
        <autoFlushInterval>5</autoFlushInterval>
    </appender>
    <root>
        <level value="ALL"/>
        <appender-ref ref="CLogAppender" />
    </root>
</log4net>
```


3. 使用示例，log4net日志的使用方式
``` 
//加载配置文件
var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/log4net.config");
XmlConfigurator.ConfigureAndWatch(logCfg);
//创建日志记录组件实例
ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//输出日志
//记录错误日志
log.Error("error", new Exception("发生了一个异常"));
//记录严重错误
log.Fatal("fatal", new Exception("发生了一个致命错误"));
//记录一般信息
log.Info("info");
//记录调试信息
log.Debug("debug");
```





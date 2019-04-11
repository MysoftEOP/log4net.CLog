# ����
log4net.CLog�ǿ���ƽ̨��Դ��log4net����־������, ���������־��Ϣ����Դ�Ƶ���־ƽ̨CLog������ʹ��log4net����¼��־��Ӧ�ã�log4net.CLog�����ã�ֻ���޸�log4net�����ļ��������κδ����޸ļ��ɽ���־�������Դ�Ƶ���־ƽ̨��

log4net.CLog����ѷ�����nuget gallery, ��ַ:https://www.nuget.org/packages/log4net.CLog

# ����ʹ��

1. ��װ���
</br>Install-Package log4net.CLog -Version 1.0.3

2. �޸�log4net�����ļ�
</br>**����˵��**
</br>appender.connectionString.server - ָ��CLog��ƽ̨����־�ӿڵ�ַ 
</br>appender.connectionString.app - ��ǰ��Ӧ�����ƣ� ����rdc
</br>appender.connectionString.identity - ��־�ı�ʶ�������Ի�����ʶ������prod/test.dev����Ҳ������������ʶ������ERP��customerId��Ϊ��ʶ, �ɸ���Ӧ�����ж��壬 ������
</br>appender.connectionString.logtype - ��־���ͣ����磬��Ϊ��־(behavior), ������־(performance), �쳣��־(exception), �ɸ���Ӧ�����ж���, ������
</br>appender.bufferSize - ��־��������С����־�������������1����ô���е���־���Ǽ�ʱ�ύ����־��ƽ̨�����������ֵ������10��������־�����ﵽ10ʱ���Ὣ10����־�����ύ����־��ƽ̨
</br>appender.autoFlushInterval - �Զ��ύ��������־�ļ��ʱ�䣬��λ���롣����ֵ��5����ôÿ��5�룬Ҳ���Զ��Ľ���������δ�ύ����־�ύ����־��ƽ̨

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


3. ʹ��ʾ����log4net��־��ʹ�÷�ʽ
``` 
//���������ļ�
var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/log4net.config");
XmlConfigurator.ConfigureAndWatch(logCfg);
//������־��¼���ʵ��
ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//�����־
//��¼������־
log.Error("error", new Exception("������һ���쳣"));
//��¼���ش���
log.Fatal("fatal", new Exception("������һ����������"));
//��¼һ����Ϣ
log.Info("info");
//��¼������Ϣ
log.Debug("debug");
```





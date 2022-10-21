# xxl-conf-sharp
## Introduction

xxl-conf-sharp is [XXL-CONF](https://github.com/xuxueli/xxl-conf )  .NET Client. support. Net 6 + framework

xxl-conf-sharp 是[XXL-CONF](https://github.com/xuxueli/xxl-conf )  .NET 客户端，支持.net 6+框架



## Tutorials 使用教程

1、Clone  Code 

克隆代码

```
git clone https://github.com/hellosnowy/xxl-conf-sharp.git
```



2、Reference to xxl_ conf_ Core in the project, refer to ApiDemo project

项目中引用 xxl_conf_core 项目，参考ApiDemo项目



3、Start the service of configuration management in the class, refer to Program.cs.

启动类中注入配置管理的服务，参考 Program.cs

```
builder.Services.AddXxlConfExecutor(builder.Configuration);
```



4、Configure the xxl conf parameter, refer to appsettings.json

配置xxl-conf参数，参考 appsettings.json

```
  "XxlConf": {
    "AdminAddress": "http://localhost:8080/xxl-conf-admin",
    "Env": "test",
    "Mirrorfile": "/data/applogs/xxl-conf/xxl-conf-mirror-sample.yaml",
    "AccessToken": ""
  }
```
5、In the class that needs to use configuration data, add the parameters of XxlConfClient to the constructor.

在需要使用配置数据的类中，构造函数中添加XxlConfClient的参数

```
 private readonly XxlConfClient confClient;
 public WeatherForecastController(XxlConfClient xxlConfClient)
 {
    confClient = xxlConfClient;  
 }
```

5、Get the configuration now

开始获取配置吧....

```
confClient.get(key)
```



6、Get the configuration changes that require dynamic monitoring, implement the XxlConfListener interface, and then add monitoring

获取需要动态监听配置变更，可实现XxlConfListener 接口，然后添加监听

```
 public class ConfigHander
    {
        private static Thread _thread;
        public ConfigHander(XxlConfClient xxlConfClient)
        {
            _thread = new Thread(t =>
            {
                xxlConfClient.addListener("default.test", new configListener());
            });
            _thread.IsBackground = true;
            _thread.Start();
        }
    }

    public class configListener : XxlConfListener
    {
        public void onChange(string key, string value)
        {
            Console.WriteLine($"配置变更事件通知：{key}={value}");
        }
    }
```

## principle实现原理

As with the official configuration, the local files are loaded into the memory first. If there is no memory, the configuration is obtained from the remote end. The architecture diagram is directly from the [official website](https://www.xuxueli.com/xxl-conf/)Got it

和官方的一样，获取配置都是先本地文件加载到内存中，内存中没有则从远端获取，架构图就直接从[官网](https://www.xuxueli.com/xxl-conf/)拿了。

![](https://github.com/hellosnowy/xxl-conf-sharp/blob/main/doc/img_08.png)

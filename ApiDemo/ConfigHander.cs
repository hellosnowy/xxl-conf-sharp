using xxl_conf_core;
using xxl_conf_core.listener;

namespace ApiDemo
{
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
}

namespace xxl_conf_core.listener
{
    /// <summary>
    /// 配置监听接口
    /// </summary>
    public interface XxlConfListener
    {
      public  void onChange(string key, string value);
    }
}

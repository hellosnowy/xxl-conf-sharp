using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using xxl_conf_core.core;

namespace xxl_conf_core.listener
{
    /// <summary>
    /// 配置监听管理器
    /// </summary>
    public class XxlConfListenerFactory
    {
        private static ConcurrentDictionary<string, List<XxlConfListener>> keyListenerRepository = new ConcurrentDictionary<string, List<XxlConfListener>>();

        private static BlockingCollection<XxlConfListener> noKeyConfListener = new BlockingCollection<XxlConfListener>();

        private readonly XxlConfLocalCacheConf xxlConfLocalCacheConf;
        private readonly ILogger<XxlConfListenerFactory> _logger;
        public XxlConfListenerFactory(ILogger<XxlConfListenerFactory> logger,
            XxlConfLocalCacheConf confLocalCacheConf)
        {
            _logger = logger;
            xxlConfLocalCacheConf = confLocalCacheConf;
        }
        public bool addListener(string key, XxlConfListener xxlConfListener)
        {
            if (xxlConfListener == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                // listene all key used
                noKeyConfListener.Add(xxlConfListener);
                return true;
            }
            else
            {
                // first use, invoke and watch this key
                try
                {
                    string value = xxlConfLocalCacheConf.get(key, null);
                    xxlConfListener.onChange(key, value);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
                if (keyListenerRepository.ContainsKey(key))
                {
                    // listene this key
                    List<XxlConfListener> listeners = keyListenerRepository[key];
                    if (listeners == null)
                    {
                        listeners = new List<XxlConfListener>();
                        keyListenerRepository.TryAdd(key, listeners);
                    }
                    listeners.Add(xxlConfListener);
                }
                else
                {
                    List<XxlConfListener> listeners = new List<XxlConfListener>();
                    listeners.Add(xxlConfListener);
                    keyListenerRepository.TryAdd(key, listeners);
                }
                return true;
            }
        }

        public void onChange(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || !keyListenerRepository.ContainsKey(key))
            {
                return;
            }
            List<XxlConfListener> keyListeners = keyListenerRepository[key];
            if (keyListeners != null && keyListeners.Count > 0)
            {
                foreach (XxlConfListener listener in keyListeners)
                {
                    try
                    {
                        listener.onChange(key, value);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }
            if (noKeyConfListener.Count > 0)
            {
                foreach (XxlConfListener confListener in noKeyConfListener)
                {
                    try
                    {
                        confListener.onChange(key, value);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }
        }
    }
}

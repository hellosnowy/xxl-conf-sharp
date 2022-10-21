using System;
using xxl_conf_core.core;
using xxl_conf_core.listener;

namespace xxl_conf_core
{
    /// <summary>
    /// 配置客户端
    /// </summary>
    public class XxlConfClient
    {
        private readonly XxlConfLocalCacheConf xxlConfLocalCacheConf;
        private readonly XxlConfListenerFactory xxlConfListenerFactory;
        public XxlConfClient(XxlConfLocalCacheConf confLocalCacheConf,
            XxlConfListenerFactory confListenerFactory)
        {
            this.xxlConfListenerFactory = confListenerFactory;
            this.xxlConfLocalCacheConf = confLocalCacheConf;
            this.xxlConfLocalCacheConf.setListenerFactory(this.xxlConfListenerFactory);
        }

        
        public string get(string key, string defaultVal)
        {
            return this.xxlConfLocalCacheConf.get(key, defaultVal);
        }

        public string get(string key)
        {
            return this.xxlConfLocalCacheConf.get(key, null);
        }


        public bool getBoolean(string key)
        {
            string value = get(key, null);
            if (value == null)
            {
                throw new Exception("config key [" + key + "] does not exist");
            }
            return bool.Parse(value);
        }

        public int getInt(string key)
        {
            string value = get(key, null);
            if (value == null)
            {
                throw new Exception("config key [" + key + "] does not exist");
            }
            return int.Parse(value);
        }

        public long getLong(string key)
        {
            string value = get(key, null);
            if (value == null)
            {
                throw new Exception("config key [" + key + "] does not exist");
            }
            return long.Parse(value);
        }

        public float getFloat(string key)
        {
            string value = get(key, null);
            if (value == null)
            {
                throw new Exception("config key [" + key + "] does not exist");
            }
            return float.Parse(value);
        }

        public double getDouble(string key)
        {
            string value = get(key, null);
            if (value == null)
            {
                throw new Exception("config key [" + key + "] does not exist");
            }
            return double.Parse(value);
        }
        public bool addListener(string key, XxlConfListener xxlConfListener)
        {
            return xxlConfListenerFactory.addListener(key, xxlConfListener);
        }

    }
}
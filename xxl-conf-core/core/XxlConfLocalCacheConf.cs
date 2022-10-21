using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using xxl_conf_core.listener;
using xxl_conf_core.utils;

namespace xxl_conf_core.core
{
    /// <summary>
    /// 本地缓存配置
    /// </summary>
    public class XxlConfLocalCacheConf
    {
        private static bool refreshThreadStop = false;
        private static Thread refreshThread;
        private readonly ILogger<XxlConfLocalCacheConf> _logger;
        private readonly XxlConfMirrorConf confMirrorConf;
        private readonly XxlConfRemoteConf confRemoteConf;
        private static XxlConfListenerFactory confListenerFactory;
        private static ConcurrentDictionary<string, string?> localCacheRepository = null;

        public XxlConfLocalCacheConf(ILogger<XxlConfLocalCacheConf> logger,
            XxlConfMirrorConf xxlConfMirrorConf,
            XxlConfRemoteConf xxlConfRemoteConf
            )
        {
            _logger = logger;
            confMirrorConf = xxlConfMirrorConf;
            confRemoteConf = xxlConfRemoteConf;

            localCacheRepository = new ConcurrentDictionary<string, string?>();
            Dictionary<string, string>? mirrorConfData = confMirrorConf.readConfMirror();
            Dictionary<string, string>? preConfData = new Dictionary<string, string>();
            Dictionary<string, string>? remoteConfData = null;
            if (mirrorConfData != null && mirrorConfData.Count > 0)
            {
                remoteConfData = confRemoteConf.find(mirrorConfData.Keys.ToList()).Result;
                DictionaryUtils.AddRange(preConfData, mirrorConfData);
                if (remoteConfData != null && remoteConfData.Count > 0)
                {
                    DictionaryUtils.AddRange(preConfData, remoteConfData);
                }
            }
            if (preConfData != null && preConfData.Count > 0)
            {
                foreach (var preKey in preConfData.Keys)
                {
                    set(preKey, preConfData[preKey], SET_TYPE.PRELOAD);
                }
            }
            refreshThread = new Thread(() =>
            {
                while (!refreshThreadStop)
                {
                    try
                    {
                        refreshCacheAndMirror();
                    }
                    catch (Exception e)
                    {
                        if (!refreshThreadStop && !(e is ThreadInterruptedException))
                        {
                            logger.LogError(">>>>>>>>>> xxl-conf, refresh thread error.");
                            logger.LogError(e.Message);
                        }
                    }

                }
            });
            refreshThread.IsBackground = true;
            refreshThread.Start();
        }

        public void setListenerFactory(XxlConfListenerFactory xxlConfListenerFactory)
        {
            confListenerFactory = xxlConfListenerFactory;
        }

        public void destroy()
        {
            if (refreshThread != null)
            {
                refreshThreadStop = true;
                refreshThread.Interrupt();
            }
        }
        public string get(string key, string defaultVal)
        {
            // level 1: local cache
            string cacheNode = get(key);
            if (cacheNode != null)
            {
                return cacheNode;
            }
            // level 2	(get-and-watch, add-local-cache)
            string? remoteData = null;
            try
            {
                remoteData = confRemoteConf.find(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            set(key, remoteData, SET_TYPE.SET);     // support cache null value
            if (remoteData != null)
            {
                return remoteData;
            }
            return defaultVal;
        }

        private string? get(string key)
        {
            if (localCacheRepository.ContainsKey(key))
            {
                return localCacheRepository[key];
            }
            return null;
        }

        private void set(string key, string? value, SET_TYPE optType)
        {
            localCacheRepository.AddOrUpdate(key, value, (k,v)=> value);
            _logger.LogInformation($">>>>>>>>>> xxl-conf: {optType}: [{key}={value}]");

            // value updated, invoke listener
            if (optType == SET_TYPE.RELOAD)
            {
                confListenerFactory.onChange(key, value);
            }

            // new conf, new monitor
            if (optType == SET_TYPE.SET)
            {
                refreshThread.Interrupt();
            }
        }

        private void refreshCacheAndMirror()
        {
            if (localCacheRepository.Count == 0)
            {
                Thread.Sleep(3000);
                return;
            }
            bool monitorRet = confRemoteConf.monitor(localCacheRepository.Keys.ToList()).Result;
            if (!monitorRet)
            {
                Thread.Sleep(10000);
            }
            // refresh cache: remote > cache
            var keySet = localCacheRepository.Keys;
            Dictionary<string, string> mirrorConfData = new Dictionary<string, string>();
            if (keySet != null && keySet.Count > 0)
            {
                Dictionary<string, string>? remoteDataMap = confRemoteConf.find(keySet.ToList()).Result;
                if (remoteDataMap != null && remoteDataMap.Count > 0)
                {
                    foreach (var remoteKey in remoteDataMap.Keys)
                    {
                        var remoteData = remoteDataMap[remoteKey];

                        var existNode = localCacheRepository[remoteKey];
                        if (!string.IsNullOrWhiteSpace(existNode) && existNode.Equals(remoteData))
                        {
                            //  logger.debug(">>>>>>>>>> xxl-conf: RELOAD unchange-pass [{}].", remoteKey);
                        }
                        else
                        {
                            set(remoteKey, remoteData, SET_TYPE.RELOAD);
                        }

                    }
                }
                // refresh mirror: cache > mirror
                foreach (var key in keySet)
                {
                    var existNode = localCacheRepository[key];
                    mirrorConfData.Add(key, existNode);
                }
            }

            confMirrorConf.writeConfMirror(mirrorConfData);

            // logger.debug(">>>>>>>>>> xxl-conf, refreshCacheAndMirror success.");
        }
    }


    public enum SET_TYPE
    {
        SET,        // first use
        RELOAD,     // value updated
        PRELOAD     // pre hot
    }
}

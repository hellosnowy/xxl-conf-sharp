using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using xxl_conf_core.core;
using xxl_conf_core.domain;
using xxl_conf_core.listener;

namespace xxl_conf_core.extensions
{
    /// <summary>
    /// 服务注册
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXxlConfExecutor(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddOptions();
            services.Configure<XxlConfFactory>(configuration.GetSection("XxlConf"));
            services.AddSingleton<XxlConfMirrorConf>();
            services.AddSingleton<XxlConfRemoteConf>();
            services.AddSingleton<XxlConfLocalCacheConf>();
            services.AddSingleton<XxlConfClient>();
            services.AddSingleton<XxlConfListenerFactory>();
            return services;
        }
       
    }
}

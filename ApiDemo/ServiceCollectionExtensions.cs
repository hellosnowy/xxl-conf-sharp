namespace ApiDemo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExecutor(this IServiceCollection services)
        {
            services.AddSingleton<ConfigHander>();
            return services;
        }
    }
}

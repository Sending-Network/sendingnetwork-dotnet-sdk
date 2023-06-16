using System.Net.Http;

namespace SDN.Sdk
{
    using Core.Domain.Services;
    using Core.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Extensions methods to configure an <see cref="IServiceCollection" /> for <see cref="IHttpClientFactory" /> with
    ///     SDN Sdk.
    /// </summary>
    public static class SDNClientServiceExtensions
    {
        public static IServiceCollection AddSDNClient(this IServiceCollection services)
        {
            services.AddSingleton<IHttpClientFactory, SingletonHttpFactory>();

            services.AddSingleton<ClientService>();
            services.AddSingleton<EventService>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<UserService>();

            services.AddTransient<IPollingService, PollingService>();
            services.AddTransient<ISDNClient, SDNClient>();

            return services;
        }
    }
}
namespace SDN.Sdk
{
    using System.Net.Http;
    using Core.Domain.Services;
    using Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;

    public class SingletonHttpFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public SingletonHttpFactory()
        {
            var httpClientHandler = new HttpClientHandler
                { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
            _httpClient = new HttpClient(httpClientHandler);
        }

        public HttpClient CreateClient(string name) => _httpClient;
    }


    public class SDNClientFactory
    {
        private readonly SingletonHttpFactory _httpClientFactory = new();
        private SDNClient? _client;

        public ISDNClient Create(ILogger<PollingService>? logger = null)
        {
            if (_client != null)
                return _client;

            var eventService = new EventService(_httpClientFactory);
            var userService = new UserService(_httpClientFactory);
            var roomService = new RoomService(_httpClientFactory);
            var pollingService = new PollingService(eventService, logger);

            _client = new SDNClient(
                pollingService,
                userService,
                roomService,
                eventService);

            return _client;
        }
    }
}
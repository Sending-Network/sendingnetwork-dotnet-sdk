namespace SDN.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.ClientVersion;
    using Extensions;

    public class ClientService : BaseApiService
    {
        public ClientService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        protected override string ResourcePath => "_api/client/r0";

        public async Task<SDNServerVersionsResponse> GetSDNClientVersions(Uri address,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient();

            return await httpClient.GetAsJsonAsync<SDNServerVersionsResponse>(ResourcePath, cancellationToken);
        }
    }
}
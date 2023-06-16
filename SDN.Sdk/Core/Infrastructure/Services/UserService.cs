// ReSharper disable ArgumentsStyleNamedExpression

namespace SDN.Sdk.Core.Infrastructure.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Login;
    using Extensions;

    public class UserService : BaseApiService
    {
        public UserService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<PreLoginResponse> PreLoginAsync(string walletAddress,
            CancellationToken cancellationToken)
        {
            var model = new PreLoginRequest("", "");

            HttpClient httpClient = CreateHttpClient();
            QueryDidResponse didResp = await httpClient.GetAsJsonAsync<QueryDidResponse>($"{ResourcePath}/address/{walletAddress}", cancellationToken);
            if(didResp.Data.Length > 0) {
                model.Did = didResp.Data[0];
            } else {
                model.Address = walletAddress;
            }

            var path = $"{ResourcePath}/did/pre_login1";

            return await httpClient.PostAsJsonAsync<PreLoginResponse>(path, model, cancellationToken);
        }

        public async Task<LoginResponse> LoginAsync(string did, string nonce, string updateTime, string message, string signature,
            CancellationToken cancellationToken)
        {
            var model = new LoginRequest
            (
                new Identifier
                (
                    did,
                    did,
                    message,
                    signature
                ),
                "m.login.did.identity",
                nonce,
                updateTime
            );

            HttpClient httpClient = CreateHttpClient();

            var path = $"{ResourcePath}/did/login";

            return await httpClient.PostAsJsonAsync<LoginResponse>(path, model, cancellationToken);
        }
    }
}
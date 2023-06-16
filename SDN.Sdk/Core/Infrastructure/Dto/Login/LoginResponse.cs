namespace SDN.Sdk.Core.Infrastructure.Dto.Login
{
    public record LoginResponse(string UserId, string AccessToken, string NodeUrl, string DeviceId)
    {
        public string UserId { get; } = UserId;
        public string AccessToken { get; } = AccessToken;
        public string NodeUrl { get; } = NodeUrl;
        public string DeviceId { get; } = DeviceId;
    }
}
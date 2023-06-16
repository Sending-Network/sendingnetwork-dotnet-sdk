namespace SDN.Sdk.Core.Infrastructure.Dto.Login
{
    public record QueryDidResponse(string[] Data)
    {
        public string[] Data { get; } = Data;
    }
}
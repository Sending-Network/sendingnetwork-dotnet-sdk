namespace SDN.Sdk.Core.Infrastructure.Dto.Login
{
    public record PreLoginResponse(string Did, string Message, string RandomServer, string Updated)
    {
        public string Did { get; } = Did;
        public string Message { get; } = Message;
        public string RandomServer { get; } = RandomServer;
        public string Updated { get; } = Updated;
    }
}
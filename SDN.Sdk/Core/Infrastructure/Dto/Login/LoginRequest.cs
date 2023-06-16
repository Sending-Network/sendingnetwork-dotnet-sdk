namespace SDN.Sdk.Core.Infrastructure.Dto.Login
{
    public record LoginRequest(Identifier Identifier, string Type, string RandomServer, string Updated)
    {
        public Identifier Identifier { get; } = Identifier;
        public string Type { get; } = Type;
        public string RandomServer { get; } = RandomServer;
        public string Updated { get; } = Updated;
    }
}
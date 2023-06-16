namespace SDN.Sdk.Core.Infrastructure.Dto.Login
{
    public record Identifier(string Did, string Address, string Message, string Token)
    {
        public string Did { get; } = Did;
        public string Address { get; } = Address;
        public string Message { get; } = Message;
        public string Token { get; } = Token;
    }
}
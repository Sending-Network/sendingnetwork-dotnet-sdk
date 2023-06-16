namespace SDN.Sdk.Core.Infrastructure.Dto.Login
{
    public record PreLoginRequest(string Address, string Did)
    {
        public string Address { get; set; } = Address;
        public string Did { get; set; } = Did;
    }
}
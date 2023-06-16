namespace SDN.Sdk.Core.Infrastructure.Dto.Room.Kick
{
    public record KickUserRequest(string UserId)
    {
        public string UserId { get; } = UserId;
    }
}
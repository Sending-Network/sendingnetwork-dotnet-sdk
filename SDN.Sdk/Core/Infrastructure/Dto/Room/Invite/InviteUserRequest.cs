namespace SDN.Sdk.Core.Infrastructure.Dto.Room.Invite
{
    public record InviteUserRequest(string UserId)
    {
        public string UserId { get; } = UserId;
    }
}
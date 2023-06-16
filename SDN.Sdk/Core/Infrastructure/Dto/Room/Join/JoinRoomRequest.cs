namespace SDN.Sdk.Core.Infrastructure.Dto.Room.Join
{
    public record JoinRoomRequest(string Reason)
    {
        public string Reason { get; } = Reason;
    }
}
namespace SDN.Sdk.Core.Domain.SDNRoom
{
    using System.Collections.Generic;

    public record SDNRoom
    {
        public SDNRoom(string id, SDNRoomStatus status, List<string> joinedUserIds)
        {
            Id = id;
            Status = status;
            JoinedUserIds = joinedUserIds;
        }

        public SDNRoom(string id, SDNRoomStatus status)
        {
            Id = id;
            Status = status;
            JoinedUserIds = new List<string>();
        }

        public string Id { get; }

        public SDNRoomStatus Status { get; }

        public List<string> JoinedUserIds { get; }
    }
}
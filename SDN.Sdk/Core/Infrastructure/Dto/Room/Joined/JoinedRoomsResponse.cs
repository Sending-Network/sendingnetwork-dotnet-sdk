namespace SDN.Sdk.Core.Infrastructure.Dto.Room.Joined
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public record JoinedRoomsResponse(List<string> JoinedRooms)
    {
        /// <summary>
        ///     <b>Required.</b> The ID of each room in which the user has joined membership.
        /// </summary>
        [JsonProperty("joined_rooms")]
        public List<string> JoinedRooms { get; } = JoinedRooms;
    }
}
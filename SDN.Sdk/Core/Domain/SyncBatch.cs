namespace SDN.Sdk.Core.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Dto.Sync;
    using SDNRoom;
    using RoomEvent;

    public record SyncBatch
    {
        private SyncBatch(string nextBatch, List<SDNRoom.SDNRoom> sdnRooms,
            List<BaseRoomEvent> sdnRoomEvents)
        {
            NextBatch = nextBatch;
            SDNRooms = sdnRooms;
            SDNRoomEvents = sdnRoomEvents;
        }

        public string NextBatch { get; }
        public List<SDNRoom.SDNRoom> SDNRooms { get; } = new();
        public List<BaseRoomEvent> SDNRoomEvents { get; } = new();

        internal static class Factory
        {
            private static readonly SDNRoomFactory SDNRoomFactory = new();
            private static readonly SDNRoomEventFactory SDNRoomEventFactory = new();

            public static SyncBatch CreateFromSync(string nextBatch, Rooms rooms)
            {
                List<SDNRoom.SDNRoom> sdnRooms = GetSDNRoomsFromSync(rooms);
                List<BaseRoomEvent> sdnRoomEvents = GetSDNEventsFromSync(rooms);

                return new SyncBatch(nextBatch, sdnRooms, sdnRoomEvents);
            }

            private static List<SDNRoom.SDNRoom> GetSDNRoomsFromSync(Rooms rooms)
            {
                var joinedSDNRooms = rooms.Join.Select(pair => SDNRoomFactory.CreateJoined(pair.Key, pair.Value))
                    .ToList();
                var invitedSDNRooms = rooms.Invite
                    .Select(pair => SDNRoomFactory.CreateInvite(pair.Key, pair.Value)).ToList();
                var leftSDNRooms = rooms.Leave.Select(pair => SDNRoomFactory.CreateLeft(pair.Key, pair.Value))
                    .ToList();

                return joinedSDNRooms.Concat(invitedSDNRooms).Concat(leftSDNRooms).ToList();
            }

            private static List<BaseRoomEvent> GetSDNEventsFromSync(Rooms rooms)
            {
                var joinedSDNRoomEvents = rooms.Join
                    .SelectMany(pair => SDNRoomEventFactory.CreateFromJoined(pair.Key, pair.Value)).ToList();
                var invitedSDNRoomEvents = rooms.Invite
                    .SelectMany(pair => SDNRoomEventFactory.CreateFromInvited(pair.Key, pair.Value)).ToList();
                var leftSDNRoomEvents = rooms.Leave
                    .SelectMany(pair => SDNRoomEventFactory.CreateFromLeft(pair.Key, pair.Value)).ToList();

                return joinedSDNRoomEvents.Concat(invitedSDNRoomEvents).Concat(leftSDNRoomEvents).ToList();
            }
        }
    }
}
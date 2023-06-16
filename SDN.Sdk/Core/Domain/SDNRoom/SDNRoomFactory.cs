namespace SDN.Sdk.Core.Domain.SDNRoom
{
    using System.Collections.Generic;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Dto.Sync.Event.Room;
    using RoomEvent;

    public class SDNRoomFactory
    {
        public SDNRoom CreateJoined(string roomId, JoinedRoom joinedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (RoomEvent timelineEvent in joinedRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out JoinRoomEvent joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new SDNRoom(roomId, SDNRoomStatus.Joined, joinedUserIds);
        }

        public SDNRoom CreateInvite(string roomId, InvitedRoom invitedRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (RoomStrippedState timelineEvent in invitedRoom.InviteState.Events)
                if (JoinRoomEvent.Factory.TryCreateFromStrippedState(timelineEvent, roomId,
                        out JoinRoomEvent joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new SDNRoom(roomId, SDNRoomStatus.Invited, joinedUserIds);
        }

        public SDNRoom CreateLeft(string roomId, LeftRoom leftRoom)
        {
            var joinedUserIds = new List<string>();
            foreach (RoomEvent timelineEvent in leftRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out JoinRoomEvent joinRoomEvent))
                    joinedUserIds.Add(joinRoomEvent!.SenderUserId);

            return new SDNRoom(roomId, SDNRoomStatus.Left, joinedUserIds);
        }
    }
}
namespace SDN.Sdk
{
    using System;
    using System.Collections.Generic;
    using Core.Domain.RoomEvent;

    public class SDNRoomEventsEventArgs : EventArgs
    {
        public SDNRoomEventsEventArgs(List<BaseRoomEvent> roomEvents, string nextBatch)
        {
            SDNRoomEvents = roomEvents;
            NextBatch = nextBatch;
        }

        public List<BaseRoomEvent> SDNRoomEvents { get; }
        
        public string NextBatch { get; }
    }
}
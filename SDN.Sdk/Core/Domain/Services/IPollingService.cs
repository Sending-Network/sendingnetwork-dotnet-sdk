namespace SDN.Sdk.Core.Domain.Services
{
    using System;
    using SDNRoom;
    
    public interface IPollingService : IDisposable
    {
        SDNRoom[] InvitedRooms { get; }

        SDNRoom[] JoinedRooms { get; }

        SDNRoom[] LeftRooms { get; }
        
        public bool IsSyncing { get; }
        
        public event EventHandler<SyncBatchEventArgs> OnSyncBatchReceived;

        void Init(Uri nodeAddress, string accessToken);

        void Start(string? nextBatch = null);

        void Stop();

        SDNRoom? GetSDNRoom(string roomId);
    }
}
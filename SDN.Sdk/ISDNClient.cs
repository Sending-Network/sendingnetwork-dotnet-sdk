namespace SDN.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Domain.SDNRoom;
    using Core.Infrastructure.Dto.Room.Create;
    using Core.Infrastructure.Dto.Room.Join;
    using Core.Infrastructure.Dto.Room.Member;
    using SDN.Sdk.Core.Infrastructure.Dto.Login;

    /// <summary>
    ///     A Client for interaction with SDN.
    /// </summary>
    public interface ISDNClient
    {
        string UserId { get; }

        Uri? BaseAddress { get; }

        bool IsLoggedIn { get; }
        
        bool IsSyncing { get; }

        SDNRoom[] InvitedRooms { get; }

        SDNRoom[] JoinedRooms { get; }

        SDNRoom[] LeftRooms { get; }
        
        event EventHandler<SDNRoomEventsEventArgs> OnSDNRoomEventsReceived;

        Task<PreLoginResponse> PreLoginAsync(Uri nodeUri, string walletAddress);

        Task<LoginResponse> LoginAsync(string did, string nonce, string updateTime, string message, string signature);

        public void SetCredential(Uri nodeUri, string userId, string accessToken);

        void Start(string? nextBatch = null);

        void Stop();

        Task<CreateRoomResponse> CreateRoomAsync(string name, string[]? invitedUserIds);

        Task<JoinRoomResponse> JoinRoomAsync(string roomId);

        Task<string> SendMessageAsync(string roomId, string message);

        Task<List<string>> GetJoinedRoomsIdsAsync();

        Task<RoomMembersResponse> GetRoomMembersAsync(string roomId);

        Task LeaveRoomAsync(string roomId);

        Task InviteUserAsync(string roomId, string userId);

        Task KickUserAsync(string roomId, string userId);
    }
}
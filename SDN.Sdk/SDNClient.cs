namespace SDN.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Domain;
    using Core.Domain.SDNRoom;
    using Core.Domain.Services;
    using Core.Infrastructure.Dto.Event;
    using Core.Infrastructure.Dto.Login;
    using Core.Infrastructure.Dto.Room.Create;
    using Core.Infrastructure.Dto.Room.Join;
    using Core.Infrastructure.Dto.Room.Joined;
    using Core.Infrastructure.Dto.Room.Member;
    using Core.Infrastructure.Services;

    /// <summary>
    ///     A Client for interaction with SDN.
    /// </summary>
    public class SDNClient : ISDNClient
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly IPollingService _pollingService;
        
        private readonly UserService _userService;
        private readonly RoomService _roomService;
        private readonly EventService _eventService;
        
       
        private string? _accessToken;
        private ulong _transactionNumber;

        public SDNClient(
            IPollingService pollingService,
            UserService userService, 
            RoomService roomService, 
            EventService eventService)
        {
            _pollingService = pollingService;
            _userService = userService;
            _roomService = roomService;
            _eventService = eventService;
        }

        public event EventHandler<SDNRoomEventsEventArgs> OnSDNRoomEventsReceived;

        public string UserId { get; private set; }

        public Uri BaseAddress { get; private set; }

        public bool IsLoggedIn { get; private set; }
        
        public bool IsSyncing { get; private set; }

        public SDNRoom[] InvitedRooms => _pollingService.InvitedRooms;
        
        public SDNRoom[] JoinedRooms => _pollingService.JoinedRooms;

        public SDNRoom[] LeftRooms => _pollingService.LeftRooms;

        public async Task<PreLoginResponse> PreLoginAsync(Uri nodeUri, string walletAddress)
        {
            _userService.BaseAddress = nodeUri;
            _roomService.BaseAddress = nodeUri;
            _eventService.BaseAddress = nodeUri;
            BaseAddress = nodeUri;
            
            PreLoginResponse response = await _userService.PreLoginAsync(walletAddress, _cts.Token);
            return response;
        }

        public async Task<LoginResponse> LoginAsync(string did, string nonce, string updateTime, string message, string signature)
        {
            LoginResponse response = await _userService.LoginAsync(did, nonce, updateTime, message, signature, _cts.Token);
            return response;
        }

        public void SetCredential(Uri nodeUri, string userId, string accessToken)
        {
            _userService.BaseAddress = nodeUri;
            _roomService.BaseAddress = nodeUri;
            _eventService.BaseAddress = nodeUri;
            BaseAddress = nodeUri;

            UserId = userId;
            _accessToken = accessToken;
            _pollingService.Init(BaseAddress, _accessToken);
            IsLoggedIn = true;
        }

        public void Start(string? nextBatch = null)
        {
            if (!IsLoggedIn)
                throw new Exception("Call LoginAsync first");

            _pollingService.OnSyncBatchReceived += OnSyncBatchReceived;
            _pollingService.Start(nextBatch);

            IsSyncing = _pollingService.IsSyncing;
        }

        public void Stop()
        {
            _pollingService.Stop();
            _pollingService.OnSyncBatchReceived -= OnSyncBatchReceived;

            IsSyncing = _pollingService.IsSyncing;
        }

        public async Task<CreateRoomResponse> CreateRoomAsync(string name, string[]? invitedUserIds) =>
            await _roomService.CreateRoomAsync(_accessToken!, name, invitedUserIds, _cts.Token);

        public async Task<JoinRoomResponse> JoinRoomAsync(string roomId)
        {
            SDNRoom? sdnRoom = _pollingService.GetSDNRoom(roomId);
            if (sdnRoom != null && sdnRoom.Status != SDNRoomStatus.Invited)
                return new JoinRoomResponse(sdnRoom.Id);

            return await _roomService.JoinRoomAsync(_accessToken!, roomId, _cts.Token);
        }

        public async Task<string> SendMessageAsync(string roomId, string message)
        {
            string transactionId = CreateTransactionId();
            
            EventResponse eventResponse = await _eventService.SendMessageAsync(_accessToken!,
                roomId, transactionId, message, _cts.Token);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
            
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            JoinedRoomsResponse response =
                await _roomService.GetJoinedRoomsAsync(_accessToken!, _cts.Token);

            return response.JoinedRooms;
        }

        public async Task<RoomMembersResponse> GetRoomMembersAsync(string roomId)
        {
            var response =
                await _roomService.GetRoomMembersAsync(_accessToken!, roomId, _cts.Token);

            return response;
        }

        public async Task LeaveRoomAsync(string roomId) => 
            await _roomService.LeaveRoomAsync(_accessToken!, roomId, _cts.Token);
        
        public async Task InviteUserAsync(string roomId, string userId) => 
            await _roomService.InviteUserAsync(_accessToken!, roomId, userId, _cts.Token);

        public async Task KickUserAsync(string roomId, string userId) => 
            await _roomService.KickUserAsync(_accessToken!, roomId, userId, _cts.Token);

        private void OnSyncBatchReceived(object? sender, SyncBatchEventArgs syncBatchEventArgs)
        {
            if (sender is not IPollingService)
                throw new ArgumentException("sender is not polling service");

            SyncBatch batch = syncBatchEventArgs.SyncBatch;

            OnSDNRoomEventsReceived.Invoke(this, new SDNRoomEventsEventArgs(batch.SDNRoomEvents,  batch.NextBatch));
        }

        private string CreateTransactionId()
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ulong counter = _transactionNumber;

            _transactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}
namespace SDN.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Services;
    using SDNRoom;
    using Microsoft.Extensions.Logging;

    public class PollingService : IPollingService
    {
        private readonly EventService _eventService;
        private readonly ILogger<PollingService>? _logger;

        private ConcurrentDictionary<string, SDNRoom> _sdnRooms;
        private CancellationTokenSource _cts;
        private string? _accessToken;
        private string _nextBatch;
        private Timer? _pollingTimer;
        private ulong _timeout;

        public PollingService(EventService eventService, ILogger<PollingService>? logger)
        {
            _eventService = eventService;
            _logger = logger;
            _timeout = Constants.FirstSyncTimout;
        }

        public bool IsSyncing { get; private set; }

        public event EventHandler<SyncBatchEventArgs> OnSyncBatchReceived;

        public SDNRoom[] InvitedRooms =>
            _sdnRooms.Values.Where(x => x.Status == SDNRoomStatus.Invited).ToArray();

        public SDNRoom[] JoinedRooms =>
            _sdnRooms.Values.Where(x => x.Status == SDNRoomStatus.Joined).ToArray();

        public SDNRoom[] LeftRooms => _sdnRooms.Values.Where(x => x.Status == SDNRoomStatus.Left).ToArray();

        public void Init(Uri nodeAddress, string accessToken)
        {
            _eventService.BaseAddress = nodeAddress;
            _accessToken = accessToken;
            _cts = new CancellationTokenSource();
            _sdnRooms = new ConcurrentDictionary<string, SDNRoom>();
            _pollingTimer = new Timer(async _ => await PollAsync());
        }

        public void Start(string? nextBatch = null)
        {
            if (_pollingTimer == null)
                throw new NullReferenceException("Call Init first.");

            if (nextBatch != null)
                _nextBatch = nextBatch;

            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
            IsSyncing = true;
        }

        public void Stop()
        {
            _cts.Cancel();
            _pollingTimer!.Change(Timeout.Infinite, Timeout.Infinite);
            IsSyncing = false;
        }

        public SDNRoom? GetSDNRoom(string roomId) =>
            _sdnRooms.TryGetValue(roomId, out SDNRoom sdnRoom) ? sdnRoom : null;

        public void Dispose()
        {
            _cts.Dispose();
            _pollingTimer?.Dispose();
        }

        private async Task PollAsync()
        {
            try
            {
                _pollingTimer!.Change(Timeout.Infinite, Timeout.Infinite);
                IsSyncing = true;

                SyncResponse response = await _eventService.SyncAsync(_accessToken!, _cts.Token,
                    _timeout, _nextBatch);
                SyncBatch syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);

                _nextBatch = syncBatch.NextBatch;
                _timeout = Constants.LaterSyncTimout;

                RefreshRooms(syncBatch.SDNRooms);
                OnSyncBatchReceived.Invoke(this, new SyncBatchEventArgs(syncBatch));

                // immediately call timer cb (this method)
                _pollingTimer?.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
            }
            catch (TaskCanceledException ex)
            {
                if (!_cts.IsCancellationRequested)
                {
                    _pollingTimer?
                        .Change(TimeSpan.FromMilliseconds(Constants.LaterSyncTimout), TimeSpan.FromMilliseconds(-1));
                }

                IsSyncing = false;
                _logger?.LogError(
                    "Polling cancelled, _cts.IsCancellationRequested {@IsCancellationRequested}:, {@Message}",
                    _cts.IsCancellationRequested, ex.Message);
            }
            catch (Exception ex)
            {
                _pollingTimer?
                    .Change(TimeSpan.FromMilliseconds(Constants.LaterSyncTimout), TimeSpan.FromMilliseconds(-1));

                IsSyncing = false;
                _logger?.LogError("Polling: exception occured. Message: {@Message}", ex.Message);
            }
        }

        private void RefreshRooms(List<SDNRoom> sdnRooms)
        {
            foreach (SDNRoom room in sdnRooms)
                if (!_sdnRooms.TryGetValue(room.Id, out SDNRoom retrievedRoom))
                {
                    if (!_sdnRooms.TryAdd(room.Id, room))
                        _logger?.LogError("Can not add sdn room");
                }
                else
                {
                    var updatedUserIds = retrievedRoom
                        .JoinedUserIds
                        .Concat(room.JoinedUserIds)
                        .Distinct()
                        .ToList();

                    var updatedRoom = new SDNRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    _sdnRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
        }
    }
}
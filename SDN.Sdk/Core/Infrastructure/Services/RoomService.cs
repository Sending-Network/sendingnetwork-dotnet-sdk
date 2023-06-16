namespace SDN.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Room.Create;
    using Dto.Room.Join;
    using Dto.Room.Joined;
    using Dto.Room.Invite;
    using Dto.Room.Kick;
    using Dto.Room.Member;
    using Extensions;

    public class RoomService : BaseApiService
    {
        public RoomService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<CreateRoomResponse> CreateRoomAsync(string accessToken, string name, string[]? members,
            CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest(Name: name, Invite: members);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/createRoom";

            return await httpClient.PostAsJsonAsync<CreateRoomResponse>(path, model, cancellationToken);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            var model = new JoinRoomRequest("");
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/join";

            return await httpClient.PostAsJsonAsync<JoinRoomResponse>(path, model, cancellationToken);
        }

        public async Task LeaveRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/leave";

            await httpClient.PostAsync(path, cancellationToken);
        }

        public async Task InviteUserAsync(string accessToken, string roomId, string userId,
            CancellationToken cancellationToken)
        {
            var model = new InviteUserRequest
            (
                UserId: userId
            );
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/invite";

            await httpClient.PostAsync(path, model, cancellationToken);
        }

        public async Task KickUserAsync(string accessToken, string roomId, string userId,
            CancellationToken cancellationToken)
        {
            var model = new KickUserRequest
            (
                UserId: userId
            );
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/kick";

            await httpClient.PostAsync(path, model, cancellationToken);
        }

        public async Task<JoinedRoomsResponse> GetJoinedRoomsAsync(string accessToken,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/joined_rooms";

            return await httpClient.GetAsJsonAsync<JoinedRoomsResponse>(path, cancellationToken);
        }

        public async Task<RoomMembersResponse> GetRoomMembersAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/joined_members";

            return await httpClient.GetAsJsonAsync<RoomMembersResponse>(path, cancellationToken);
        }
    }
}
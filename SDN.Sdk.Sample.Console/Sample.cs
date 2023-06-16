namespace SDN.Sdk.Sample.Console
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Core.Domain.RoomEvent;
    using Microsoft.Extensions.DependencyInjection;
    using Nethereum.Signer;
    using Newtonsoft.Json;

    public class Sample
    {   
        public async Task Run(IServiceProvider serviceProvider)
        {

            var nodeUrl = ConfigurationManager.AppSettings.Get("NodeUrl");
            var walletAddress = ConfigurationManager.AppSettings.Get("WalletAddress");
            var privateKey = ConfigurationManager.AppSettings.Get("PrivateKey");
            var userId = ConfigurationManager.AppSettings.Get("UserId");
            var accessToken = ConfigurationManager.AppSettings.Get("AccessToken");
            if(nodeUrl == null || walletAddress == null || privateKey == null)
            {
                Console.WriteLine("config value not found");
                return;
            }

            var client = serviceProvider.GetRequiredService<ISDNClient>();
            if(userId == null || accessToken == null) {
                // pre login
                var preLoginResponse = await client.PreLoginAsync(new Uri(nodeUrl), walletAddress);

                // sign message
                var signer = new EthereumMessageSigner();
                var signature = signer.EncodeUTF8AndSign(preLoginResponse.Message, new EthECKey(privateKey));

                // login
                var loginResponse = await client.LoginAsync(preLoginResponse.Did, preLoginResponse.RandomServer, preLoginResponse.Updated, preLoginResponse.Message, signature);
                userId = loginResponse.UserId;
                accessToken = loginResponse.AccessToken;

                SaveCredential(userId, accessToken);
            }
            client.SetCredential(new Uri(nodeUrl), userId, accessToken);

            // add event listener
            client.OnSDNRoomEventsReceived += (sender, eventArgs) =>
            {
                foreach (var roomEvent in eventArgs.SDNRoomEvents)
                {
                    if (roomEvent is not TextMessageEvent textMessageEvent)
                        continue;

                    (string roomId, string senderUserId, string message) = textMessageEvent;
                    if (client.UserId != senderUserId)
                        Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
                }
            };

            // start syncing
            client.Start();

            // process commands
            while (true)
            {
                Console.Write("[Input command]");
                var input = Console.ReadLine();
                if (input == null) {
                    continue;
                }
                if (input.ToLower() == "q"){
                    break; // if 'q'，exit
                }
                var parameters = input.Split(' ');
                if (parameters[0] != "room") {
                    continue;
                }
                var action = parameters[1];
                switch (action)
                {
                    case "list":
                        var joinedRoomIds = await client.GetJoinedRoomsIdsAsync();
                        Console.WriteLine("Joined rooms: [{0}]", string.Join(",", joinedRoomIds));
                        break;
                    case "create":
                        var roomName = parameters[2];
                        var createRoomResponse = await client.CreateRoomAsync(roomName, null);
                        Console.WriteLine($"Created new room: {createRoomResponse.RoomId}");
                        break;
                    case "invite":
                        var inviteRoomId = parameters[2];
                        var inviteUserId = parameters[3];
                        await client.InviteUserAsync(inviteRoomId, inviteUserId);
                        Console.WriteLine("Invite success.");
                        break;
                    case "kick":
                        var kickRoomId = parameters[2];
                        var kickUserId = parameters[3];
                        await client.KickUserAsync(kickRoomId, kickUserId);
                        Console.WriteLine("Kick success.");
                        break;
                    case "join":
                        var joinRoomId = parameters[2];
                        await client.JoinRoomAsync(joinRoomId);
                        Console.WriteLine("Join success.");
                        break;
                    case "leave":
                        var leaveRoomId = parameters[2];
                        await client.LeaveRoomAsync(leaveRoomId);
                        Console.WriteLine("Leave success.");
                        break;
                    case "members":
                        var roomId = parameters[2];
                        var resp = await client.GetRoomMembersAsync(roomId);
                        Console.WriteLine("Room members: {0}", JsonConvert.SerializeObject(resp.Joined));
                        break;
                    case "send":
                        var sendRoomId = parameters[2];
                        var sendMessage = parameters[3];
                        var eventId = await client.SendMessageAsync(sendRoomId, sendMessage);
                        Console.WriteLine($"Send success! EventId: {eventId}");
                        break;
                    default:
                        PrintHelp();
                        break;
                }
            }

            client.Stop();
            Console.WriteLine("Program exited.");
        }

        void SaveCredential(string userId, string accessToken)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if(Array.Exists(config.AppSettings.Settings.AllKeys, e => e == "UserId")) {
                config.AppSettings.Settings["UserId"].Value = userId;
            } else {
                config.AppSettings.Settings.Add("UserId", userId);
            }
            if(Array.Exists(config.AppSettings.Settings.AllKeys, e => e == "AccessToken")) {
                config.AppSettings.Settings["AccessToken"].Value = accessToken;
            } else {
                config.AppSettings.Settings.Add("AccessToken", accessToken);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        void PrintHelp()
        {
            Console.WriteLine(
@"room list -- List room
room create <roomName> -- Create new room
room members <roomId> -- List room members
room invite <roomId> <userId> -- Invite user to room
room send <roomId> <message text> -- Send a text message
room join <roomId> -- Join room by id");
        }
    }
}
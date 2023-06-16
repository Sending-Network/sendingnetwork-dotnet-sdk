# SendingNetwork DotNet SDK

A DotNet SDK for SendingNetwork.

## Supported Platforms

* [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) or greater

## Installation

Add dependency
```shell
dotnet add package SDN.Sdk
```

## Use the SDK in your app
### 1. Create 
Use `SDNClientFactory` to create an instance of `SDNClient`
```csharp
var factory = new SDNClientFactory();
ISDNClient client = factory.Create();
```

### 2.Login
Currently, SDNClient supports only wallet account login
```csharp
// pre login
var preLoginResponse = await client.PreLoginAsync(new Uri(nodeUrl), walletAddress);

// sign message (or prompt user to sign with a wallet app)
var signer = new EthereumMessageSigner();
var signature = signer.EncodeUTF8AndSign(preLoginResponse.Message, new EthECKey(privateKey));

// login did
var loginResponse = await client.LoginAsync(preLoginResponse.Did, preLoginResponse.RandomServer, preLoginResponse.Updated, preLoginResponse.Message, signature);
                userId = loginResponse.UserId;
                accessToken = loginResponse.AccessToken;

// set client credential
client.SetCredential(new Uri(nodeUrl), loginResponse.UserId, loginResponse.AccessToken);
```

### 3. Start listening for incomming events
To listen for the incoming room events you need to subscribe to `OnSDNRoomEventsReceived;`
```csharp
// add event listener
client.OnSDNRoomEventsReceived += (sender, eventArgs) =>
{
    foreach (var roomEvent in eventArgs.SDNRoomEvents)
    {
        if (roomEvent is TextMessageEvent textMessageEvent)
        {
            Console.WriteLine($"RoomId: {textMessageEvent.RoomId} received event from {textMessageEvent.SenderUserId}: {textMessageEvent.Message}.");
        }
    }
};

// start syncing
client.Start();
```

### 4. Call API functions
```csharp
// create new room
await client.CreateRoomAsync(roomName, null);

// invite user to the room
await client.InviteUserAsync(roomId, userId);

// send room message
await client.SendMessageAsync(roomId, "hello");
```

## Examples
For a complete example, refer to `Sample.cs`.You can also clone this repository and run `SDN.Sdk.Sample.Console`.
namespace SDN.Sdk.Core.Infrastructure.Dto.Room.Member
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public record RoomMembersResponse(Dictionary<string, MemberInfo> Joined)
    {
        public Dictionary<string, MemberInfo> Joined { get; } = Joined;
    }

    public record MemberInfo(string DisplayName, string AvatarUrl)
    {
        public string DisplayName { get; } = DisplayName;
        public string AvatarUrl { get; } = AvatarUrl;
    }
}
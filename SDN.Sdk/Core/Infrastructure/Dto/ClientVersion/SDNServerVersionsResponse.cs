namespace SDN.Sdk.Core.Infrastructure.Dto.ClientVersion
{
    using System.Collections.Generic;

    public class SDNServerVersionsResponse
    {
        public List<string> versions { get; set; }
        public UnstableFeatures unstable_features { get; set; }
    }
}
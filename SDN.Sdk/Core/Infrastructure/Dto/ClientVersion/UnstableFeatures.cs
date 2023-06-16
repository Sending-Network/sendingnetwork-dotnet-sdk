namespace SDN.Sdk.Core.Infrastructure.Dto.ClientVersion
{
    using Newtonsoft.Json;

    public class UnstableFeatures
    {
        [JsonProperty("org.sdn.label_based_filtering")]
        public bool OrgSDNLabelBasedFiltering { get; set; }

        [JsonProperty("org.sdn.e2e_cross_signing")]
        public bool OrgSDNE2eCrossSigning { get; set; }

        [JsonProperty("org.sdn.msc2432")] public bool OrgSDNMsc2432 { get; set; }

        [JsonProperty("uk.half-shot.msc2666")] public bool UkHalfShotMsc2666 { get; set; }

        [JsonProperty("io.element.e2ee_forced.public")]
        public bool IoElementE2eeForcedPublic { get; set; }

        [JsonProperty("io.element.e2ee_forced.private")]
        public bool IoElementE2eeForcedPrivate { get; set; }

        [JsonProperty("io.element.e2ee_forced.trusted_private")]
        public bool IoElementE2eeForcedTrustedPrivate { get; set; }

        [JsonProperty("org.sdn.msc3026.busy_presence")]
        public bool OrgSDNMsc3026BusyPresence { get; set; }
    }
}
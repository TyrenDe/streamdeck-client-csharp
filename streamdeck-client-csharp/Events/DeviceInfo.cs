using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class DeviceInfo
    {
        [JsonProperty("type")]
        public DeviceType Type { get; private set; }

        [JsonProperty("size")]
        public DeviceSize Size { get; private set; }
    }
}

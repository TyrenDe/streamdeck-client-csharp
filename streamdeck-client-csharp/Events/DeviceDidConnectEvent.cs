using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class DeviceDidConnectEvent : BaseEvent
    {
        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("deviceInfo")]
        public DeviceInfo DeviceInfo { get; private set; }
    }
}

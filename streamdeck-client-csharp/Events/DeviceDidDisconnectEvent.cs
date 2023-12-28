using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class DeviceDidDisconnectEvent : BaseEvent
    {
        [JsonProperty("device")]
        public string Device { get; private set; }
    }
}

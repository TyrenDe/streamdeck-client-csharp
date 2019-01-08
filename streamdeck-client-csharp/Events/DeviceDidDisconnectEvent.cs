using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class DeviceDidDisconnectEvent : BaseEvent
    {
        [JsonProperty("device")]
        public string Device { get; private set; }
    }
}

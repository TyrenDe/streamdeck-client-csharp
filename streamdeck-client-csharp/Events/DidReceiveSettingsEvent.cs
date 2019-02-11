using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class DidReceiveSettingsEvent : BaseEvent
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public ReceiveSettingsPayload Payload { get; private set; }
    }
}

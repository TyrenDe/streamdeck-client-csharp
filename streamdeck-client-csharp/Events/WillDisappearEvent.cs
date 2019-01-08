using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class WillDisappearEvent : BaseEvent
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public AppearancePayload Payload { get; private set; }
    }
}

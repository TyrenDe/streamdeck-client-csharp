using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class KeyUpEvent : BaseEvent
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public KeyPayload Payload { get; private set; }
    }
}

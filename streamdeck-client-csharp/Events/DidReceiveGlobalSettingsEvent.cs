using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class DidReceiveGlobalSettingsEvent : BaseEvent
    {
        [JsonProperty("payload")]
        public ReceiveGlobalSettingsPayload Payload { get; private set; }
    }
}

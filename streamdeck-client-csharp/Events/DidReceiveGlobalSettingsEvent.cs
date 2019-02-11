using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class DidReceiveGlobalSettingsEvent : BaseEvent
    {
        [JsonProperty("payload")]
        public ReceiveGlobalSettingsPayload Payload { get; private set; }
    }
}

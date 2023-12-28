using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class ApplicationDidTerminateEvent : BaseEvent
    {
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }
    }
}

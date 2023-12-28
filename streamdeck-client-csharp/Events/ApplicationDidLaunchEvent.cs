using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class ApplicationDidLaunchEvent : BaseEvent
    {
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }
    }
}

using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class ApplicationDidTerminateEvent : BaseEvent
    {
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }
    }
}

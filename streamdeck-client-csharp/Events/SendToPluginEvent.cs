using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace streamdeck_client_csharp.Events
{
    public class SendToPluginEvent : BaseEvent
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public JObject Payload { get; private set; }
    }
}

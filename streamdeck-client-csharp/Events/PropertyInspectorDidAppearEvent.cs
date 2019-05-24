using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace streamdeck_client_csharp.Events
{
    public class PropertyInspectorDidAppearEvent : BaseEvent
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }
    }
}

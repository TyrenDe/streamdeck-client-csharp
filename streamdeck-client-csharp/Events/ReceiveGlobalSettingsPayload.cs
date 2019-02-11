using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace streamdeck_client_csharp.Events
{
    public class ReceiveGlobalSettingsPayload
    {
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }
    }
}

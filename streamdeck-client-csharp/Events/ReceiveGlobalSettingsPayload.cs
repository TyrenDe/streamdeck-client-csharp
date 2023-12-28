using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamDeck.Client.Events
{
    public class ReceiveGlobalSettingsPayload
    {
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }
    }
}

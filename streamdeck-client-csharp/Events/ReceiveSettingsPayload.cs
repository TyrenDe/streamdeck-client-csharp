using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamDeck.Client.Events
{
    public class ReceiveSettingsPayload
    {
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; private set; }

        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }
    }
}

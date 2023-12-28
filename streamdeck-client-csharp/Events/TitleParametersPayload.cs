using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamDeck.Client.Events
{
    public class TitleParametersPayload
    {
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; private set; }

        [JsonProperty("state")]
        public uint State { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("titleParameters")]
        public TitleParameters TitleParameters { get; private set; }
    }
}

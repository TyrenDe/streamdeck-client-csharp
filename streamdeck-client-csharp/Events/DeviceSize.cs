using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class DeviceSize
    {
        [JsonProperty("columns")]
        public int Columns { get; private set; }

        [JsonProperty("rows")]
        public int Rows { get; private set; }
    }
}

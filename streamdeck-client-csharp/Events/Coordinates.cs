using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class Coordinates
    {
        [JsonProperty("column")]
        public int Columns { get; private set; }

        [JsonProperty("row")]
        public int Rows { get; private set; }
    }
}

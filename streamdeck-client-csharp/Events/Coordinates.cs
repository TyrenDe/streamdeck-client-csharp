using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class Coordinates
    {
        [JsonProperty("column")]
        public int Columns { get; private set; }

        [JsonProperty("row")]
        public int Rows { get; private set; }
    }
}

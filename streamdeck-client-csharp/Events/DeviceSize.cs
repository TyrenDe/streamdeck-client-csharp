using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class DeviceSize
    {
        [JsonProperty("columns")]
        public int Columns { get; private set; }

        [JsonProperty("rows")]
        public int Rows { get; private set; }
    }
}

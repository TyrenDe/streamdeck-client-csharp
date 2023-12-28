using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class ApplicationPayload
    {
        [JsonProperty("application")]
        public string Application { get; private set; }
    }
}

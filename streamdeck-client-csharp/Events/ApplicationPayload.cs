using Newtonsoft.Json;

namespace streamdeck_client_csharp.Events
{
    public class ApplicationPayload
    {
        [JsonProperty("application")]
        public string Application { get; private set; }
    }
}

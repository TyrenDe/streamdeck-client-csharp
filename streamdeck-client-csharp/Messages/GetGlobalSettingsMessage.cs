using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace streamdeck_client_csharp.Messages
{
    internal class GetGlobalSettingsMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "getGlobalSettings"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        public GetGlobalSettingsMessage(string context)
        {
            this.Context = context;
        }
    }
}

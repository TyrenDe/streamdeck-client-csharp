using Newtonsoft.Json;

namespace streamdeck_client_csharp.Messages
{
    internal class ShowOkMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "showOk"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        public ShowOkMessage(string context)
        {
            this.Context = context;
        }
    }
}

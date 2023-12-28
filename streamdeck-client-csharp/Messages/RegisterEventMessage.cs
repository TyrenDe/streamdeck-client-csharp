using Newtonsoft.Json;
using System;

namespace StreamDeck.Client.Messages
{
    internal class RegisterEventMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get; private set; }

        [JsonProperty("uuid")]
        public string UUID { get; private set; }

        public RegisterEventMessage(string eventName, string uuid)
        {
            this.Event = eventName;
            this.UUID = uuid;
        }
    }
}

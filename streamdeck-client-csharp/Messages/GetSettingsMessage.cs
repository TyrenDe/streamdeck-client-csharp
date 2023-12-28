﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamDeck.Client.Messages
{
    internal class GetSettingsMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "getSettings"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        public GetSettingsMessage(string context)
        {
            this.Context = context;
        }
    }
}

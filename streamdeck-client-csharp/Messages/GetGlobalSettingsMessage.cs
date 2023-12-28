﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamDeck.Client.Messages
{
    internal class GetGlobalSettingsMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "getGlobalSettings"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        public GetGlobalSettingsMessage(string pluginUUID)
        {
            this.Context = pluginUUID;
        }
    }
}

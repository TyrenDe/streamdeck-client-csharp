﻿using Newtonsoft.Json;

namespace StreamDeck.Client.Messages
{
    internal class ShowAlertMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "showAlert"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        public ShowAlertMessage(string context)
        {
            this.Context = context;
        }
    }
}

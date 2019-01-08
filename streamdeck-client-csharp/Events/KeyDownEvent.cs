using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace streamdeck_client_csharp.Events
{
    public class KeyDownEvent : BaseEvent
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public KeyPayload Payload { get; private set; }
    }
}

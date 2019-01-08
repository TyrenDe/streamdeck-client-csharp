using Newtonsoft.Json;

namespace streamdeck_client_csharp.Messages
{
    internal class SwitchToProfileMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "switchToProfile"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SwitchToProfileMessage(string device, string profileName, string context)
        {
            this.Context = context;
            this.Device = device;
            this.Payload = new PayloadClass(profileName);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("profile")]
            public string Profile { get; private set; }

            public PayloadClass(string profile)
            {
                this.Profile = profile;
            }
        }
    }
}

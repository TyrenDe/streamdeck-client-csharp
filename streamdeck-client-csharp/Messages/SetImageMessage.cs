using Newtonsoft.Json;

namespace streamdeck_client_csharp.Messages
{
    internal class SetImageMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setImage"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SetImageMessage(string base64Image, string context, SDKTarget target)
        {
            this.Context = context;
            this.Payload = new PayloadClass(base64Image, target);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("image")]
            public string Image { get; private set; }

            [JsonProperty("target")]
            public SDKTarget Target { get; private set; }

            public PayloadClass(string image, SDKTarget target)
            {
                this.Image = image;
                this.Target = target;
            }
        }
    }
}

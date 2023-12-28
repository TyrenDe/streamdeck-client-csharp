using Newtonsoft.Json;

namespace StreamDeck.Client.Events
{
    public class TitleParameters
    {
        [JsonProperty("fontFamily")]
        public string FontFamily { get; private set; }

        [JsonProperty("fontSize")]
        public uint FontSize { get; private set; }

        [JsonProperty("fontStyle")]
        public string FontStyle { get; private set; }

        [JsonProperty("fontUnderline")]
        public bool FontUnderline { get; private set; }

        [JsonProperty("showTitle")]
        public bool ShowTitle { get; private set; }

        [JsonProperty("titleAlignment")]
        public string TitleAlignment { get; private set; }

        [JsonProperty("titleColor")]
        public string TitleColor { get; private set; }
    }
}

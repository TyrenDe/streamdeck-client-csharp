using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace streamdeck_client_csharp.Events
{
    public abstract class BaseEvent
    {
        private static Dictionary<string, Type> s_EventMap = new Dictionary<string, Type>
        {
            { "keyDown", typeof(KeyDownEvent) },
            { "keyUp", typeof(KeyUpEvent) },

            { "willAppear", typeof(WillAppearEvent) },
            { "willDisappear", typeof(WillDisappearEvent) },

            { "titleParametersDidChange", typeof(TitleParametersDidChangeEvent) },

            { "deviceDidConnect", typeof(DeviceDidConnectEvent) },
            { "deviceDidDisconnect", typeof(DeviceDidDisconnectEvent) },

            { "applicationDidLaunch", typeof(ApplicationDidLaunchEvent) },
            { "applicationDidTerminate", typeof(ApplicationDidTerminateEvent) },
        };

        [JsonProperty("event")]
        public string Event { get; set; }

        internal static BaseEvent Parse(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            if (!jsonObject.ContainsKey("event"))
            {
                throw new InvalidOperationException("Invalid message, missing `event` type");
            }

            string eventType = jsonObject["event"].ToString();
            if (!s_EventMap.ContainsKey(eventType))
            {
                throw new InvalidOperationException($"Invalid message, unknown `event` type: {eventType}");
            }

            return JsonConvert.DeserializeObject(json, s_EventMap[eventType]) as BaseEvent;
        }
    }
}

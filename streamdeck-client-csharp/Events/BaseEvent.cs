using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace streamdeck_client_csharp.Events
{
    public static class EventTypes
    {
        public const string KeyDown = "keyDown";
        public const string KeyUp = "keyUp";
        public const string WillAppear = "willAppear";
        public const string WillDisappear = "willDisappear";
        public const string TitleParametersDidChange = "titleParametersDidChange";
        public const string DeviceDidConnect = "deviceDidConnect";
        public const string DeviceDidDisconnect = "deviceDidDisconnect";
        public const string ApplicationDidLaunch = "applicationDidLaunch";
        public const string ApplicationDidTerminate = "applicationDidTerminate";
        public const string SendToPlugin = "sendToPlugin";
    }

    public abstract class BaseEvent
    {
        private static Dictionary<string, Type> s_EventMap = new Dictionary<string, Type>
        {
            { EventTypes.KeyDown, typeof(KeyDownEvent) },
            { EventTypes.KeyUp, typeof(KeyUpEvent) },

            { EventTypes.WillAppear, typeof(WillAppearEvent) },
            { EventTypes.WillDisappear, typeof(WillDisappearEvent) },

            { EventTypes.TitleParametersDidChange, typeof(TitleParametersDidChangeEvent) },

            { EventTypes.DeviceDidConnect, typeof(DeviceDidConnectEvent) },
            { EventTypes.DeviceDidDisconnect, typeof(DeviceDidDisconnectEvent) },

            { EventTypes.ApplicationDidLaunch, typeof(ApplicationDidLaunchEvent) },
            { EventTypes.ApplicationDidTerminate, typeof(ApplicationDidTerminateEvent) },

            { EventTypes.SendToPlugin, typeof(SendToPluginEvent) },
        };

        [JsonProperty("event")]
        public string Event { get; set; }

        internal static BaseEvent Parse(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            if (!jsonObject.ContainsKey("event"))
            {
                return null;
            }

            string eventType = jsonObject["event"].ToString();
            if (!s_EventMap.ContainsKey(eventType))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(json, s_EventMap[eventType]) as BaseEvent;
        }
    }
}

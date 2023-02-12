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
        public const string SystemDidWakeUp = "systemDidWakeUp";
        public const string DidReceiveSettings = "didReceiveSettings";
        public const string DidReceiveGlobalSettings = "didReceiveGlobalSettings";
        public const string PropertyInspectorDidAppear = "propertyInspectorDidAppear";
        public const string PropertyInspectorDidDisappear = "propertyInspectorDidDisappear";
        public const string SendToPlugin = "sendToPlugin";
        public const string DialRotate = "dialRotate";
        public const string DialPress = "dialPress";
        public const string TouchpadPress = "touchTap";
    }

    public abstract class BaseEvent
    {
        private static readonly Dictionary<string, Type> s_EventMap = new Dictionary<string, Type>
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

            { EventTypes.SystemDidWakeUp, typeof(SystemDidWakeUpEvent) },

            { EventTypes.DidReceiveSettings, typeof(DidReceiveSettingsEvent) },
            { EventTypes.DidReceiveGlobalSettings, typeof(DidReceiveGlobalSettingsEvent) },

            { EventTypes.PropertyInspectorDidAppear, typeof(PropertyInspectorDidAppearEvent) },
            { EventTypes.PropertyInspectorDidDisappear, typeof(PropertyInspectorDidDisappearEvent) },

            { EventTypes.SendToPlugin, typeof(SendToPluginEvent) },

            { EventTypes.DialRotate, typeof(DialRotateEvent) },
            { EventTypes.DialPress, typeof(DialPressEvent) },
            { EventTypes.TouchpadPress, typeof(TouchpadPressEvent) },
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

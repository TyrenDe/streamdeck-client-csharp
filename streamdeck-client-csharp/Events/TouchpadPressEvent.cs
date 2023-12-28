﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.Client.Events
{
    /// <summary>
    /// Payload for touchpad press
    /// </summary>
    public class TouchpadPressEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Unique Action UUID
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device UUID key was pressed on
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Information on touchpad press
        /// </summary>
        [JsonProperty("payload")]
        public TouchpadPressPayload Payload { get; private set; }
    }
}

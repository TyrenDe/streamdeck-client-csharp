using StreamDeck.Client.Events;
using System;

namespace StreamDeck.Client
{
    public class StreamDeckEventReceivedEventArgs<T> : EventArgs
    {
        public T Event { get; private set; }
        internal StreamDeckEventReceivedEventArgs(T evt)
        {
            this.Event = evt;
        }
    }
}

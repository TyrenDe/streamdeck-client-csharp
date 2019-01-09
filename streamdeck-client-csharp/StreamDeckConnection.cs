using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using streamdeck_client_csharp.Events;
using streamdeck_client_csharp.Messages;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace streamdeck_client_csharp
{
    public class StreamDeckConnection
    {
        private const int BufferSize = 1024 * 1024;

        private ClientWebSocket m_WebSocket;
        private SemaphoreSlim m_SendSocketSemaphore = new SemaphoreSlim(1);
        private CancellationTokenSource m_CancelSource = new CancellationTokenSource();
        private string m_RegisterEvent;

        /// <summary>
        /// The port used to connect to the StreamDeck websocket
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// This is the unique identifier used to communicate with the register StreamDeck plugin.
        /// </summary>
        public string UUID { get; private set; }

        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;

        public event EventHandler<StreamDeckEventReceivedEventArgs<KeyDownEvent>> OnKeyDown;
        public event EventHandler<StreamDeckEventReceivedEventArgs<KeyUpEvent>> OnKeyUp;
        public event EventHandler<StreamDeckEventReceivedEventArgs<WillAppearEvent>> OnWillAppear;
        public event EventHandler<StreamDeckEventReceivedEventArgs<WillDisappearEvent>> OnWillDisappear;
        public event EventHandler<StreamDeckEventReceivedEventArgs<TitleParametersDidChangeEvent>> OnTitleParametersDidChange;
        public event EventHandler<StreamDeckEventReceivedEventArgs<DeviceDidConnectEvent>> OnDeviceDidConnect;
        public event EventHandler<StreamDeckEventReceivedEventArgs<DeviceDidDisconnectEvent>> OnDeviceDidDisconnect;
        public event EventHandler<StreamDeckEventReceivedEventArgs<ApplicationDidLaunchEvent>> OnApplicationDidLaunch;
        public event EventHandler<StreamDeckEventReceivedEventArgs<ApplicationDidTerminateEvent>> OnApplicationDidTerminate;
        public event EventHandler<StreamDeckEventReceivedEventArgs<JObject>> OnGenericEvent;

        public StreamDeckConnection(int port, string uuid, string registerEvent)
        {
            this.Port = port;
            this.UUID = uuid;
            m_RegisterEvent = registerEvent;
        }

        public void Run()
        {
            if (m_WebSocket == null)
            {
                m_WebSocket = new ClientWebSocket();
                _ = this.RunAsync();
            }
        }

        public void Stop()
        {
            m_CancelSource.Cancel();
        }

        public Task SetTitleAsync(string title, string context, SDKTarget target)
        {
            return SendAsync(new SetTitleMessage(title, context, target));
        }

        public Task SetImageAsync(Image image, string context, SDKTarget target)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();

                // Convert byte[] to Base64 String
                string base64String = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
                return SetImageAsync(base64String, context, target);
            }
        }

        public Task SetImageAsync(string base64Image, string context, SDKTarget target)
        {
            return SendAsync(new SetImageMessage(base64Image, context, target));
        }

        public Task ShowAlertAsync(string context)
        {
            return SendAsync(new ShowAlertMessage(context));
        }

        public Task ShowOkAsync(string context)
        {
            return SendAsync(new ShowOkMessage(context));
        }

        public Task SetSettingsAsync(JObject settings, string context)
        {
            return SendAsync(new SetSettingsMessage(settings, context));
        }

        public Task SetStateAsync(uint state, string context)
        {
            return SendAsync(new SetStateMessage(state, context));
        }

        public Task SendToPropertyInspectorAsync(string action, JObject data, string context)
        {
            return SendAsync(new SendToPropertyInspectorMessage(action, data, context));
        }

        public Task SwitchToProfileAsync(string device, string profileName, string context)
        {
            return SendAsync(new SwitchToProfileMessage(device, profileName, context));
        }
        public Task OpenUrlAsync(string uri)
        {
            return OpenUrlAsync(new Uri(uri));
        }

        public Task OpenUrlAsync(Uri uri)
        {
            return SendAsync(new OpenUrlMessage(uri));
        }

        private Task SendAsync(IMessage message)
        {
            return SendAsync(JsonConvert.SerializeObject(message));
        }

        private async Task SendAsync(string text)
        {
            try
            {
                if (m_WebSocket != null)
                {
                    try
                    {
                        await m_SendSocketSemaphore.WaitAsync();
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        await m_WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, m_CancelSource.Token);
                    }
                    finally
                    {
                        m_SendSocketSemaphore.Release();
                    }
                }
            }
            catch
            {
                await DisconnectAsync();
            }
        }

        private async Task RunAsync()
        {
            try
            {
                await m_WebSocket.ConnectAsync(new Uri($"ws://localhost:{this.Port}"), m_CancelSource.Token);
                if (m_WebSocket.State != WebSocketState.Open)
                {
                    await DisconnectAsync();
                    return;
                }

                await SendAsync(new RegisterEventMessage(m_RegisterEvent, this.UUID));

                OnConnected?.Invoke(this, new EventArgs());
                await ReceiveAsync();
            }
            finally
            {
                await DisconnectAsync();
            }
        }

        private async Task<WebSocketCloseStatus> ReceiveAsync()
        {
            byte[] buffer = new byte[BufferSize];
            ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(buffer);
            StringBuilder textBuffer = new StringBuilder(BufferSize);

            try
            {
                while (!m_CancelSource.IsCancellationRequested && m_WebSocket != null)
                {
                    WebSocketReceiveResult result = await m_WebSocket.ReceiveAsync(arrayBuffer, m_CancelSource.Token);

                    if (result != null)
                    {
                        if (result.MessageType == WebSocketMessageType.Close ||
                            (result.CloseStatus != null && result.CloseStatus.HasValue && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                        {
                            return result.CloseStatus.GetValueOrDefault();
                        }
                        else if (result.MessageType == WebSocketMessageType.Text)
                        {
                            textBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                            if (result.EndOfMessage)
                            {
                                BaseEvent evt = BaseEvent.Parse(textBuffer.ToString());
                                if (evt == null)
                                {
                                    OnGenericEvent?.Invoke(this, new StreamDeckEventReceivedEventArgs<JObject>(JObject.Parse(textBuffer.ToString())));
                                }
                                else
                                {
                                    switch (evt.Event)
                                    {
                                        case EventTypes.KeyDown: OnKeyDown?.Invoke(this, new StreamDeckEventReceivedEventArgs<KeyDownEvent>(evt as KeyDownEvent)); break;
                                        case EventTypes.KeyUp: OnKeyUp?.Invoke(this, new StreamDeckEventReceivedEventArgs<KeyUpEvent>(evt as KeyUpEvent)); break;
                                        case EventTypes.WillAppear: OnWillAppear?.Invoke(this, new StreamDeckEventReceivedEventArgs<WillAppearEvent>(evt as WillAppearEvent)); break;
                                        case EventTypes.WillDisappear: OnWillDisappear?.Invoke(this, new StreamDeckEventReceivedEventArgs<WillDisappearEvent>(evt as WillDisappearEvent)); break;
                                        case EventTypes.TitleParametersDidChange: OnTitleParametersDidChange?.Invoke(this, new StreamDeckEventReceivedEventArgs<TitleParametersDidChangeEvent>(evt as TitleParametersDidChangeEvent)); break;
                                        case EventTypes.DeviceDidConnect: OnDeviceDidConnect?.Invoke(this, new StreamDeckEventReceivedEventArgs<DeviceDidConnectEvent>(evt as DeviceDidConnectEvent)); break;
                                        case EventTypes.DeviceDidDisconnect: OnDeviceDidDisconnect?.Invoke(this, new StreamDeckEventReceivedEventArgs<DeviceDidDisconnectEvent>(evt as DeviceDidDisconnectEvent)); break;
                                        case EventTypes.ApplicationDidLaunch: OnApplicationDidLaunch?.Invoke(this, new StreamDeckEventReceivedEventArgs<ApplicationDidLaunchEvent>(evt as ApplicationDidLaunchEvent)); break;
                                        case EventTypes.ApplicationDidTerminate: OnApplicationDidTerminate?.Invoke(this, new StreamDeckEventReceivedEventArgs<ApplicationDidTerminateEvent>(evt as ApplicationDidTerminateEvent)); break;
                                    }
                                }

                                textBuffer.Clear();
                            }
                        }
                    }
                }
            }
            catch { }

            return WebSocketCloseStatus.NormalClosure;
        }

        private async Task DisconnectAsync()
        {
            if (m_WebSocket != null)
            {
                ClientWebSocket socket = m_WebSocket;
                m_WebSocket = null;

                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", m_CancelSource.Token);
                }
                catch { }

                try
                {
                    socket.Dispose();
                }
                catch { }

                OnDisconnected?.Invoke(this, new EventArgs());
            }
        }
    }
}

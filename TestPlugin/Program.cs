using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using streamdeck_client_csharp;
using streamdeck_client_csharp.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPlugin
{
    class Program
    {
        public class Options
        {
            [Option("port", Required = true, HelpText = "The websocket port to connect to", SetName = "port")]
            public int Port { get; set; }

            [Option("pluginUUID", Required = true, HelpText = "The UUID of the plugin")]
            public string PluginUUID { get; set; }

            [Option("registerEvent", Required = true, HelpText = "The event triggered when the plugin is registered?")]
            public string RegisterEvent { get; set; }

            [Option("info", Required = true, HelpText = "Extra JSON launch data")]
            public string Info { get; set; }
        }

        // StreamDeck launches the plugin with these details
        // -port [number] -pluginUUID [GUID] -registerEvent [string?] -info [json]
        static void Main(string[] args)
        {
            // Uncomment this line of code to allow for debugging
            // while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            // The command line args parser expects all args to use `--`, so, let's append
            for (int count = 0; count < args.Length; count++)
            {
                if (args[count].StartsWith("-") && !args[count].StartsWith("--"))
                {
                    args[count] = $"-{args[count]}";
                }
            }

            Parser parser = new Parser((with) =>
            {
                with.EnableDashDash = true;
                with.CaseInsensitiveEnumValues = true;
                with.CaseSensitive = false;
                with.IgnoreUnknownArguments = true;
                with.HelpWriter = Console.Error;
            });

            ParserResult<Options> options = parser.ParseArguments<Options>(args);
            options.WithParsed<Options>(o => RunPlugin(o));
        }

        static void RunPlugin(Options options)
        {
            ManualResetEvent connectEvent = new ManualResetEvent(false);
            ManualResetEvent disconnectEvent = new ManualResetEvent(false);

            StreamDeckConnection connection = new StreamDeckConnection(options.Port, options.PluginUUID, options.RegisterEvent);

            connection.OnConnected += (sender, args) =>
            {
                connectEvent.Set();
            };

            connection.OnDisconnected += (sender, args) =>
            {
                disconnectEvent.Set();
            };

            connection.OnApplicationDidLaunch += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"App Launch: {args.Event.Payload.Application}");
            };

            connection.OnApplicationDidTerminate += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"App Terminate: {args.Event.Payload.Application}");
            };

            Dictionary<string, int> counters = new Dictionary<string, int>();
            List<string> images = new List<string>();
            Dictionary<string, JObject> settings = new Dictionary<string, JObject>();
            connection.OnWillAppear += (sender, args) =>
            {
                switch (args.Event.Action)
                {
                    case "com.tyren.testplugin.counter":
                        lock (counters)
                        {
                            counters[args.Event.Context] = 0;
                        }
                        break;
                    case "com.tyren.testplugin.changeimage":
                        lock (images)
                        {
                            images.Add(args.Event.Context);
                        }
                        break;
                    case "com.tyren.testplugin.pidemo":
                        lock (settings)
                        {
                            settings[args.Event.Context] = args.Event.Payload.Settings;
                            if (settings[args.Event.Context] == null)
                            {
                                settings[args.Event.Context] = new JObject();
                            }
                            if (settings[args.Event.Context]["selectedValue"] == null)
                            {
                                settings[args.Event.Context]["selectedValue"] = JValue.CreateString("20");
                            }
                            if (settings[args.Event.Context]["textDemoValue"] == null)
                            {
                                settings[args.Event.Context]["textDemoValue"] = JValue.CreateString("");
                            }
                        }
                        break;
                }
            };

            connection.OnWillDisappear += (sender, args) =>
            {
                lock (counters)
                {
                    if (counters.ContainsKey(args.Event.Context))
                    {
                        counters.Remove(args.Event.Context);
                    }
                }

                lock (images)
                {
                    if (images.Contains(args.Event.Context))
                    {
                        images.Remove(args.Event.Context);
                    }
                }

                lock (settings)
                {
                    if (settings.ContainsKey(args.Event.Context))
                    {
                        settings.Remove(args.Event.Context);
                    }
                }
            };

            connection.OnSendToPlugin += async (sender, args) =>
            {
                JObject setting = null;
                switch (args.Event.Payload["property_inspector"].ToString().ToLower())
                {
                    case "propertyinspectorconnected":
                        // Send settings to Property Inspector
                        lock (settings)
                        {
                            settings.TryGetValue(args.Event.Context, out setting);
                        }

                        if (setting != null)
                        {
                            await connection.SendToPropertyInspectorAsync(args.Event.Action, setting, args.Event.Context);
                        }
                        break;
                    case "propertyinspectorwilldisappear":
                        lock (settings)
                        {
                            settings.TryGetValue(args.Event.Context, out setting);
                        }

                        if (setting != null)
                        {
                            await connection.SetSettingsAsync(setting, args.Event.Context);
                        }
                        break;
                    case "updatesettings":
                        lock (settings)
                        {
                            settings.TryGetValue(args.Event.Context, out setting);
                        }

                        if (setting != null)
                        {
                            setting["selectedValue"] = args.Event.Payload["selectedValue"];
                            setting["textDemoValue"] = args.Event.Payload["textDemoValue"];
                            await connection.SetSettingsAsync(setting, args.Event.Context);
                        }
                        break;
                }



                if (setting != null)
                {

                }
            };

            // Start the connection
            connection.Run();

            // Current Directory is the base Stream Deck Install path.
            // For example: C:\Program Files\Elgato\StreamDeck\
            Image image = Image.FromFile(@"Plugins\com.tyren.testplugin.sdPlugin\Images\TyDidIt40x40.png");

            // Wait for up to 10 seconds to connect
            if (connectEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                // We connected, loop every second until we disconnect
                while (!disconnectEvent.WaitOne(TimeSpan.FromMilliseconds(1000)))
                {
                    lock (counters)
                    {
                        foreach (KeyValuePair<string, int> kvp in counters.ToArray())
                        {
                            _ = connection.SetTitleAsync(kvp.Value.ToString(), kvp.Key, SDKTarget.HardwareAndSoftware);
                            counters[kvp.Key]++;
                        }
                    }

                    lock (images)
                    {
                        foreach (string imageContext in images)
                        {
                            _ = connection.SetImageAsync(image, imageContext, SDKTarget.HardwareAndSoftware);
                        }

                        images.Clear();
                    }
                }
            }
        }
    }
}

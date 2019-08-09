using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace F12019New
{
    class Globals
    {
        public static string f1_2019_dll_name = "F12019NewTelemetryProvider";
        public static bool is_f1_2019 = true;
    }

    public class F12019NewTelemetryProvider : AbstractTelemetryProvider
    {
        private bool isStopped = true;                  // flag to control the polling thread
        private Thread t;                               // the polling thread, reads telemetry data and sends TelemetryUpdated events
        private const int PORTNUM = 20777;              // Server Port
        private IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread


        public F12019NewTelemetryProvider() : base()
        {
            Author = "ffxf";
            Version = "v0.1";
            BannerImage = @"img\simfeedback.png"; // Image shown on top of the profiles tab
            IconImage = @"img\codemasters.png";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 60;       // the update frequency in samples per second
        }

        public override string Name { get { return "F12019New"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing F12019NewTelemetryProvider");
        }

        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(F12019NewTelemetryData));
        }

        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting F12019NewTelemetryProvider");
                isStopped = false;
                t = new Thread(Run);
                t.Start();
            }
        }

        public override void Stop()
        {
            if (!isStopped)
            {
                LogDebug("Stopping F12019NewTelemetryProvider");
                isStopped = true;
                if (t != null) t.Join();
            }
        }

        private void Run()
        {
            F12019NewTelemetryData data;
            PacketMotionData packData;

            int bytesInMotionPacket = 1343; // Valid for F1-2019
            LogDebug($"Provider Assembly Name: '{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}'");
            if (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name != Globals.f1_2019_dll_name)
            {
                LogDebug("Running as F1-2018 Telemetry Provider");
                bytesInMotionPacket = 1341;
                Globals.is_f1_2019 = false;
            } else
                LogDebug("Running as F1-2019 Telemetry Provider");

            Session session = new Session();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), PORTNUM));

            Log("Listener started (port: " + PORTNUM.ToString() + ") F12019NewTelemetryProvider.Thread");

            while (!isStopped)
            {
                try
                {
                    // get data from game, 
                    if (socket.Available == 0)
                    {
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            IsRunning = false;
                            IsConnected = false;
                            Thread.Sleep(1000);
                        }
                        continue;
                    }
                    else
                    {
                        IsConnected = true;
                    }

                    
                    Byte[] received = socket.Receive(ref _senderIP);
                    
                    if (received.Length == bytesInMotionPacket)
                    {
                        packData = new PacketMotionData(received);
                        
                        if (packData.m_header.m_packetId == 0) // should always be the case if the # of bytes received match
                        {
                            data = new F12019NewTelemetryData(packData);
                            
                            IsRunning = true;

                            TelemetryEventArgs args = new TelemetryEventArgs(
                                new F12019NewTelemetryInfo(data, session));
                            RaiseEvent(OnTelemetryUpdate, args);
                        }
                        else
                        {
                            LogDebug("Received packet has expected number of bytes but is not a motion packet. This should not happen.");
                            IsRunning = false;
                        }
                    }
                    
                    sw.Restart();
                    //Thread.Sleep(SamplePeriod);


                }
                catch (Exception e)
                {
                    IsConnected = false;
                    IsRunning = false;
                    LogDebug($"Caught Exception while unmarshalling data '{e.Message}'");
                    Thread.Sleep(1000);
                }
            }

            socket.Close();
            IsConnected = false;
            IsRunning = false;
        }
    }
}

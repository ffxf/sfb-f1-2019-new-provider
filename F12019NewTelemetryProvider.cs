using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace F12019New
{
    public class F12019NewTelemetryProvider : AbstractTelemetryProvider
    {
        public static UInt16 lastRPMs = 0;
        public static int bytesInMotionPacket2019 = 1343;       // Valid for F1-2019; -2 for 2018
        public static int bytesInTelemetryPacket2019 = 1347;    // Valid for F1-2019; -2 for 2018

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

                    if (received.Length == bytesInTelemetryPacket2019 || received.Length == bytesInTelemetryPacket2019-2)
                    {
                        var telemData = new PacketCarTelemetryData(received);
                        lastRPMs = telemData.m_carTelemetryData[0].m_engineRPM;
                    } else if (received.Length == bytesInMotionPacket2019 || received.Length == bytesInMotionPacket2019-2)
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
                            lastRPMs = 0;
                        }
                    }
                    
                    sw.Restart();
                    //Thread.Sleep(SamplePeriod);


                }
                catch (Exception e)
                {
                    IsConnected = false;
                    IsRunning = false;
                    lastRPMs = 0;
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

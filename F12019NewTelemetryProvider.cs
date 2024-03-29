﻿// F1-2018/2019 Telemetry Provider for SimFeedback supporting the "new" UDP format for both games

using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace F12019New
{
    public class F12019NewTelemetryProvider : AbstractTelemetryProvider
    {
        public static UInt16 lastRPMs = 0;                      // Contains last RPM value from car telemetry package
        public static int spectatorCarIndex = 0;                // Returned by Session data as the driver being watched
        public static int humanDriver = 0;                      // Returned by Participant data as the driver being human controlled
        public static int bytesInMotionPacket2019 = 1343;       // Valid for F1-2019; -2 (=1341) for 2018
        public static int bytesInTelemetryPacket2019 = 1347;    // Valid for F1-2019
        public static int bytesInTelemetryPacket2018 = 1085;    // Valid for F1-2018
        public static int bytesInParticipantPacket2019 = 1104;  // Valid for F1-2019
        public static int bytesInParticipantPacket2018 = 1082;  // Valid for F1-2018
        public static int bytesInSessionData2019 = 149;         // Valid for F1-2019
        public static int bytesInSessionData2018 = 147;         // Valid for F1-2018

        private bool isStopped = true;                  // flag to control the polling thread
        private Thread t;                               // the polling thread, reads telemetry data and sends TelemetryUpdated events
        private int PORTNUM = 20777;                    // Server Port; may get reset via file <SFB_root>/provider/f12019new_port
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

            var portFile = @"provider\f12019new_port.txt"; // Optional file to define f1 game telemetry port
            if (File.Exists(portFile))
            {
                LogDebug($"F12019New port number file '{portFile}' was found - trying to set port defined in there.");
                var port = File.ReadAllText(portFile);
                if (Int32.TryParse(port, out int iport))
                    PORTNUM = iport;
            }
            LogDebug($"F1-2018 or 2019 telemetry port number is {PORTNUM}");
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
            F12019NewTelemetryData data;    // Exposes only data for interest to SimFeedback
            PacketMotionData packData;      // Raw data as received from game

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
                    LogDebug($"Packet size: '{received.Length}'");

                    if (received.Length == bytesInTelemetryPacket2019 || received.Length == bytesInTelemetryPacket2018)
                    {
                        // Unfortunately we need to "digest" the car telemetry data just to get the RPMs out of it.
                        // Storing the RPMs in class variable for easier access when adding it to our SFB exposed telemetry data
                        var telemData = new PacketCarTelemetryData(received);
                        if (received.Length == bytesInTelemetryPacket2019)
                            lastRPMs = telemData.m_carTelemetryData2019[0].m_engineRPM;
                        else
                            lastRPMs = telemData.m_carTelemetryData2018[0].m_engineRPM;
                        LogDebug($"Got RPMs: '{lastRPMs}'");
                    }
                    else if (received.Length == bytesInMotionPacket2019 || received.Length == bytesInMotionPacket2019 - 2)
                    {
                        packData = new PacketMotionData(received);

                        if (packData.m_header.m_packetId == 0) // should always be the case if the # of bytes received match
                        {
                            data = new F12019NewTelemetryData(packData);

                            LogDebug($"Got longitudal g force: '{data.gForceLongitudinal}'");

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
                    else if (received.Length == bytesInParticipantPacket2019 || received.Length == bytesInParticipantPacket2018)
                    {
                        var participantData = new ParticpantData(received);
                        LogDebug($"Got human driver index '{participantData.humanDriver}'");
                        humanDriver = participantData.humanDriver;
                    }
                    else if (received.Length == bytesInSessionData2019 || received.Length == bytesInSessionData2018)
                    {
                        var sessionData = new SessionData(received);
                        LogDebug($"Got watched driver index '{sessionData.m_spectatorCarIndex}'");
                        spectatorCarIndex = sessionData.m_spectatorCarIndex;
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

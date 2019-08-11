// The car telemetry data for F1-2018/2019. While the entries are mostly the same for both games (with one
// additional entry for F1-2019, which is the surface type array) the data types
// are different for some elements - so we need different classes for each game.
// We can tell them apart by the length of the incoming byte array.

using System;

namespace F12019New
{
    class CarTelemetryData2019
    {
        public int sz { get; set; }

        public UInt16 m_speed;                                  // Speed of car in kilometres per hour
        public float m_throttle;                                // Amount of throttle applied (0.0 to 1.0)
        public float m_steer;                                   // Steering (-1.0 (full lock left) to 1.0 (full lock right))
        public float m_brake;                                   // Amount of brake applied (0.0 to 1.0)
        public byte m_clutch;                                   // Amount of clutch applied (0 to 100)
        public sbyte m_gear;                                    // Gear selected (1-8, N=0, R=-1)
        public UInt16 m_engineRPM;                              // Engine RPM
        public byte m_drs;                                      // 0 = off, 1 = on
        public byte m_revLightsPercent;                         // Rev lights indicator (percentage)
        public UInt16[] m_brakesTemperature { get; set; }       // Brakes temperature (celsius)
        public UInt16[] m_tyresSurfaceTemperature { get; set; } // Tyres surface temperature (celsius)
        public UInt16[] m_tyresInnerTemperature { get; set; }   // Tyres inner temperature (celsius)
        public UInt16 m_engineTemperature { get; set; }         // Engine temperature (celsius)
        public float[] m_tyresPressure { get; set; }            // Tyres pressure (PSI)
        public byte[] m_surfaceType { get; set; }               // Driving surface, see appendices

        public CarTelemetryData2019(byte[] rawData, int idx)
        {
            int start_idx = idx;

            this.m_speed = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_throttle = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_steer = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_brake = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_clutch = rawData[idx]; idx += 1;
            this.m_gear = (sbyte)rawData[idx]; idx += 1;
            this.m_engineRPM = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_drs = rawData[idx]; idx += 1;
            this.m_revLightsPercent = rawData[idx]; idx += 1;
            this.m_brakesTemperature = DataConverter.ConvArray<UInt16>(rawData, idx, 4, BitConverter.ToUInt16); idx += 4 * sizeof(UInt16);
            this.m_tyresSurfaceTemperature = DataConverter.ConvArray<UInt16>(rawData, idx, 4, BitConverter.ToUInt16); idx += 4 * sizeof(UInt16);
            this.m_tyresInnerTemperature = DataConverter.ConvArray<UInt16>(rawData, idx, 4, BitConverter.ToUInt16); idx += 4 * sizeof(UInt16);
            this.m_engineTemperature = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_tyresPressure = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);
            this.m_surfaceType = DataConverter.ConvArray<byte>(rawData, idx, 4, (rd, i) => { return rd[i]; }); idx += 4 * sizeof(byte);

            this.sz = idx - start_idx;
        }
    }

    // If you compare to the 2019 format then throttle/steer/break/clutch are floats there while only byte values
    // for 2018 plus the surface array (the surface under each wheel) is missing in 2018
    class CarTelemetryData2018
    {
        public int sz { get; set; }

        public UInt16 m_speed;                                  // Speed of car in kilometres per hour
        public byte m_throttle;                                 // Amount of throttle applied (0.0 to 1.0)
        public sbyte m_steer;                                   // Steering (-1.0 (full lock left) to 1.0 (full lock right))
        public byte m_brake;                                    // Amount of brake applied (0.0 to 1.0)
        public byte m_clutch;                                   // Amount of clutch applied (0 to 100)
        public sbyte m_gear;                                    // Gear selected (1-8, N=0, R=-1)
        public UInt16 m_engineRPM;                              // Engine RPM
        public byte m_drs;                                      // 0 = off, 1 = on
        public byte m_revLightsPercent;                         // Rev lights indicator (percentage)
        public UInt16[] m_brakesTemperature { get; set; }       // Brakes temperature (celsius)
        public UInt16[] m_tyresSurfaceTemperature { get; set; } // Tyres surface temperature (celsius)
        public UInt16[] m_tyresInnerTemperature { get; set; }   // Tyres inner temperature (celsius)
        public UInt16 m_engineTemperature { get; set; }         // Engine temperature (celsius)
        public float[] m_tyresPressure { get; set; }            // Tyres pressure (PSI)

        public CarTelemetryData2018(byte[] rawData, int idx)
        {
            int start_idx = idx;

            this.m_speed = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_throttle = rawData[idx]; idx += 1;
            this.m_steer = (sbyte)rawData[idx]; idx += 1;
            this.m_brake = rawData[idx]; idx += 1;
            this.m_clutch = rawData[idx]; idx += 1;
            this.m_gear = (sbyte)rawData[idx]; idx += 1;
            this.m_engineRPM = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_drs = rawData[idx]; idx += 1;
            this.m_revLightsPercent = rawData[idx]; idx += 1;
            this.m_brakesTemperature = DataConverter.ConvArray<UInt16>(rawData, idx, 4, BitConverter.ToUInt16); idx += 4 * sizeof(UInt16);
            this.m_tyresSurfaceTemperature = DataConverter.ConvArray<UInt16>(rawData, idx, 4, BitConverter.ToUInt16); idx += 4 * sizeof(UInt16);
            this.m_tyresInnerTemperature = DataConverter.ConvArray<UInt16>(rawData, idx, 4, BitConverter.ToUInt16); idx += 4 * sizeof(UInt16);
            this.m_engineTemperature = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_tyresPressure = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);

            this.sz = idx - start_idx;
        }
    }

    class PacketCarTelemetryData
    {
        public PacketHeader m_header;                              // Header
        public CarTelemetryData2018[] m_carTelemetryData2018 { get; set; } // Data for all cars on track
        public CarTelemetryData2019[] m_carTelemetryData2019 { get; set; } // Data for all cars on track
        public UInt32 m_buttonStatus { get; set; }          // Bit flags specifying which buttons are being pressed
                                                            // currently - see appendices
        public PacketCarTelemetryData(byte[] rawData)
        {
            int idx = 0;
            this.m_header = new PacketHeader(rawData, idx); idx += this.m_header.sz;
            if (rawData.Length == F12019NewTelemetryProvider.bytesInTelemetryPacket2018)
            {
                this.m_carTelemetryData2018 = new CarTelemetryData2018[20];
                // We unpack only car 0 for performance reasons because it is all we need right now
                this.m_carTelemetryData2018[0] = new CarTelemetryData2018(rawData, idx); idx += 20 * this.m_carTelemetryData2018[0].sz;
                // Comment out the above and uncomment the below if you need data from other cars
                //for (int i = 0; i < 20; i++)
                //{
                //    this.m_carTelemetryData2018[i] = new CarTelemetryData2018(rawData, idx); idx += this.m_carTelemetryData2018[i].sz;
                //}
            }
            else
            {
                this.m_carTelemetryData2019 = new CarTelemetryData2019[20];
                // We unpack only car 0 for performance reasons because it is all we need right now
                this.m_carTelemetryData2019[0] = new CarTelemetryData2019(rawData, idx); idx += 20 * this.m_carTelemetryData2019[0].sz;
                // Comment out the above and uncomment the below if you need data from other cars
                //for (int i = 0; i < 20; i++)
                //{
                //    this.m_carTelemetryData2019[i] = new CarTelemetryData2019(rawData, idx); idx += this.m_carTelemetryData2019[i].sz;
                //}
            }
        }
    }
}

using System;

namespace F12019New
{
    class PacketHeader
    {
        public int sz;

        public UInt16 m_packetFormat { get; set; }         // 2019
        public byte m_gameMajorVersion { get; set; }       // Game major version - "X.00"
        public byte m_gameMinorVersion { get; set; }       // Game minor version - "1.XX"
        public byte m_packetVersion { get; set; }          // Version of this packet type, all start from 1
        public byte m_packetId { get; set; }               // Identifier for the packet type, see below
        public UInt64 m_sessionUID { get; set; }           // Unique identifier for the session
        public float m_sessionTime { get; set; }           // Session timestamp
        public uint m_frameIdentifier { get; set; }        // Identifier for the frame the data was retrieved on
        public byte m_playerCarIndex { get; set; }         // Index of player's car in the array

        public PacketHeader(byte[] rawData, int idx)
        {
            int start_idx = idx;

            this.m_packetFormat = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            if (rawData.Length == F12019NewTelemetryProvider.bytesInMotionPacket2019 ||
                rawData.Length == F12019NewTelemetryProvider.bytesInTelemetryPacket2019)
            {
                this.m_gameMajorVersion = rawData[idx]; idx += 1;
                this.m_gameMinorVersion = rawData[idx]; idx += 1;
            }
            else
            {
                this.m_gameMajorVersion = 0;
                this.m_gameMinorVersion = 0;
            }
            this.m_packetVersion = rawData[idx]; idx += 1;
            this.m_packetId = rawData[idx]; idx += 1;
            this.m_sessionUID = BitConverter.ToUInt64(rawData, idx); idx += sizeof(UInt64);
            this.m_sessionTime = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_frameIdentifier = BitConverter.ToUInt32(rawData, idx); idx += sizeof(UInt32);
            this.m_playerCarIndex = rawData[idx]; idx += 1;

            this.sz = idx - start_idx;
        }
    }

    class CarMotionData
    {
        public int sz;

        public float m_worldPositionX { get; set; }           // World space X position
        public float m_worldPositionY { get; set; }           // World space Y position
        public float m_worldPositionZ { get; set; }           // World space Z position
        public float m_worldVelocityX { get; set; }           // Velocity in world space X
        public float m_worldVelocityY { get; set; }           // Velocity in world space Y
        public float m_worldVelocityZ { get; set; }           // Velocity in world space Z
        public Int16 m_worldForwardDirX { get; set; }         // World space forward X direction (normalised)
        public Int16 m_worldForwardDirY { get; set; }         // World space forward Y direction (normalised)
        public Int16 m_worldForwardDirZ { get; set; }         // World space forward Z direction (normalised)
        public Int16 m_worldRightDirX { get; set; }           // World space right X direction (normalised)
        public Int16 m_worldRightDirY { get; set; }           // World space right Y direction (normalised)
        public Int16 m_worldRightDirZ { get; set; }           // World space right Z direction (normalised)
        public float m_gForceLateral { get; set; }            // Lateral G-Force component
        public float m_gForceLongitudinal { get; set; }       // Longitudinal G-Force component
        public float m_gForceVertical { get; set; }           // Vertical G-Force component
        public float m_yaw { get; set; }                      // Yaw angle in radians
        public float m_pitch { get; set; }                    // Pitch angle in radians
        public float m_roll { get; set; }                     // Roll angle in radians

        public CarMotionData(byte[] rawData, int idx)
        {
            int start_idx = idx;

            this.m_worldPositionX = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_worldPositionY = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_worldPositionZ = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_worldVelocityX = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_worldVelocityY = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_worldVelocityZ = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_worldForwardDirX = BitConverter.ToInt16(rawData, idx); idx += sizeof(Int16);
            this.m_worldForwardDirY = BitConverter.ToInt16(rawData, idx); idx += sizeof(Int16);
            this.m_worldForwardDirZ = BitConverter.ToInt16(rawData, idx); idx += sizeof(Int16);
            this.m_worldRightDirX = BitConverter.ToInt16(rawData, idx); idx += sizeof(Int16);
            this.m_worldRightDirY = BitConverter.ToInt16(rawData, idx); idx += sizeof(Int16);
            this.m_worldRightDirZ = BitConverter.ToInt16(rawData, idx); idx += sizeof(Int16);
            this.m_gForceLateral = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_gForceLongitudinal = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_gForceVertical = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_yaw = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_pitch = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_roll = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);

            this.sz = idx - start_idx;
        }
    }

    class PacketMotionData
    {
        public PacketHeader m_header;                        // Header
        public CarMotionData[] m_carMotionData { get; set; } // Data for all cars on track

        // Extra player car ONLY data
        public float[] m_suspensionPosition { get; set; }     // Note: All wheel arrays have the following order:
        public float[] m_suspensionVelocity { get; set; }     // RL, RR, FL, FR
        public float[] m_suspensionAcceleration { get; set; } // RL, RR, FL, FR
        public float[] m_wheelSpeed { get; set; }             // Speed of each wheel
        public float[] m_wheelSlip { get; set; }              // Slip ratio for each wheel
        public float m_localVelocityX;                        // Velocity in local space
        public float m_localVelocityY;                        // Velocity in local space
        public float m_localVelocityZ;                        // Velocity in local space
        public float m_angularVelocityX;                      // Angular velocity x-component
        public float m_angularVelocityY;                      // Angular velocity y-component
        public float m_angularVelocityZ;                      // Angular velocity z-component
        public float m_angularAccelerationX;                  // Angular velocity x-component
        public float m_angularAccelerationY;                  // Angular velocity y-component
        public float m_angularAccelerationZ;                  // Angular velocity z-component
        public float m_frontWheelsAngle;                      // Current front wheels angle in radians

        public PacketMotionData(byte[] rawData)
        {
            int idx = 0;
            this.m_header = new PacketHeader(rawData, idx); idx += this.m_header.sz;
            this.m_carMotionData = new CarMotionData[20];
            for (int i = 0; i < 20; i++)
            {
                this.m_carMotionData[i] = new CarMotionData(rawData, idx); idx += this.m_carMotionData[i].sz;
            }
            this.m_suspensionPosition = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);
            this.m_suspensionVelocity = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);
            this.m_suspensionAcceleration = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);
            this.m_wheelSpeed = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);
            this.m_wheelSlip = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);
            this.m_localVelocityX = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_localVelocityY = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_localVelocityZ = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_angularVelocityX = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_angularVelocityY = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_angularVelocityZ = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_angularAccelerationX = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_angularAccelerationY = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_angularAccelerationZ = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
            this.m_frontWheelsAngle = BitConverter.ToInt32(rawData, idx); idx += sizeof(float);
        }
    }

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
            this.m_brakesTemperature = ArrayConverter.Conv4Array<UInt16>(rawData, idx); idx += 4 * sizeof(UInt16);
            this.m_tyresSurfaceTemperature = ArrayConverter.Conv4Array<UInt16>(rawData, idx); idx += 4 * sizeof(UInt16);
            this.m_tyresInnerTemperature = ArrayConverter.Conv4Array<UInt16>(rawData, idx); idx += 4 * sizeof(UInt16);
            this.m_engineTemperature = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_tyresPressure = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);
            this.m_surfaceType = ArrayConverter.Conv4Array<byte>(rawData, idx); idx += 4 * sizeof(byte);

            this.sz = idx - start_idx;
        }
    }

    class CarTelemetryData2018
    {
        public int sz { get; set; }

        public UInt16 m_speed;                                  // Speed of car in kilometres per hour
        public byte m_throttle;                                // Amount of throttle applied (0.0 to 1.0)
        public sbyte m_steer;                                   // Steering (-1.0 (full lock left) to 1.0 (full lock right))
        public byte m_brake;                                   // Amount of brake applied (0.0 to 1.0)
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
            this.m_brakesTemperature = ArrayConverter.Conv4Array<UInt16>(rawData, idx); idx += 4 * sizeof(UInt16);
            this.m_tyresSurfaceTemperature = ArrayConverter.Conv4Array<UInt16>(rawData, idx); idx += 4 * sizeof(UInt16);
            this.m_tyresInnerTemperature = ArrayConverter.Conv4Array<UInt16>(rawData, idx); idx += 4 * sizeof(UInt16);
            this.m_engineTemperature = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_tyresPressure = ArrayConverter.Conv4Array<float>(rawData, idx); idx += 4 * sizeof(float);

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
                for (int i = 0; i < 20; i++)
                {
                    this.m_carTelemetryData2018[i] = new CarTelemetryData2018(rawData, idx); idx += this.m_carTelemetryData2018[i].sz;
                }
            } else
            {
                this.m_carTelemetryData2019 = new CarTelemetryData2019[20];
                for (int i = 0; i < 20; i++)
                {
                    this.m_carTelemetryData2019[i] = new CarTelemetryData2019(rawData, idx); idx += this.m_carTelemetryData2019[i].sz;
                }
            }
        }
    }

    class F12019NewTelemetryData
    {
        public float posX { get; set; }           // World space X position
        public float posY { get; set; }           // World space Y position
        public float posZ { get; set; }           // World space Z position
        public float worldVelocityX { get; set; }           // Velocity in world space X
        public float worldVelocityY { get; set; }           // Velocity in world space Y
        public float worldVelocityZ { get; set; }           // Velocity in world space Z
        public float localVelocityX { get; set; }
        public float localVelocityY { get; set; }
        public float localVelocityZ { get; set; }
        public float gForceLateral { get; set; }            // Lateral G-Force component
        public float gForceLongitudinal { get; set; }       // Longitudinal G-Force component
        public float gForceVertical { get; set; }           // Vertical G-Force component
        public float yaw { get; set; }                      // Yaw angle in radians
        public float pitch { get; set; }                    // Pitch angle in radians
        public float roll { get; set; }                     // Roll angle in radians
        public float wheelSlipRL { get; set; }
        public float wheelSlipRR { get; set; }
        public float wheelSlipFL { get; set; }
        public float wheelSlipFR { get; set; }
        public float suspAccelRL { get; set; }
        public float suspAccelRR { get; set; }
        public float suspAccelFL { get; set; }
        public float suspAccelFR { get; set; }

        public float angularVelocityX { get; set; }                      // Angular velocity x-component
        public float angularVelocityY { get; set; }                      // Angular velocity y-component
        public float angularVelocityZ { get; set; }                      // Angular velocity z-component
        public float angularAccelerationX { get; set; }                  // Angular velocity x-component
        public float angularAccelerationY { get; set; }                  // Angular velocity y-component
        public float angularAccelerationZ { get; set; }                  // Angular velocity z-component
        public float frontWheelsAngle { get; set; }                      // Current front wheels angle in radians
        public float TractionLoss { get; set; }                          // Derived value. Set in F12019NewTelemetryInfo.cs
        public float BumpRight { get; set; }                             // Derived value. Set in F12019NewTelemetryInfo.cs
        public float BumpLeft { get; set; }                              // Derived value. Set in F12019NewTelemetryInfo.cs
        public int RPMs { get; set; }                                    // From telemetry data packets stored in lastRPMs

        public F12019NewTelemetryData(PacketMotionData data)
        {
            this.posX = data.m_carMotionData[0].m_worldPositionX;
            this.posY = data.m_carMotionData[0].m_worldPositionY;
            this.posZ = data.m_carMotionData[0].m_worldPositionZ;
            this.worldVelocityX = data.m_carMotionData[0].m_worldVelocityX;
            this.worldVelocityY = data.m_carMotionData[0].m_worldVelocityY;
            this.worldVelocityZ = data.m_carMotionData[0].m_worldVelocityZ;
            this.localVelocityX = data.m_localVelocityX;
            this.localVelocityY = data.m_localVelocityY;
            this.localVelocityZ = data.m_localVelocityZ;
            this.gForceLateral = data.m_carMotionData[0].m_gForceLateral;
            this.gForceLongitudinal = data.m_carMotionData[0].m_gForceLongitudinal;
            this.gForceVertical = data.m_carMotionData[0].m_gForceVertical;
            this.yaw = data.m_carMotionData[0].m_yaw;
            this.pitch = data.m_carMotionData[0].m_pitch;
            this.roll = data.m_carMotionData[0].m_roll;
            this.wheelSlipRL = data.m_wheelSlip[0];
            this.wheelSlipRR = data.m_wheelSlip[1];
            this.wheelSlipFL = data.m_wheelSlip[2];
            this.wheelSlipFR = data.m_wheelSlip[3];
            this.suspAccelRL = data.m_suspensionAcceleration[0];
            this.suspAccelRR = data.m_suspensionAcceleration[1];
            this.suspAccelFL = data.m_suspensionAcceleration[2];
            this.suspAccelFR = data.m_suspensionAcceleration[3];
            this.angularVelocityX = data.m_angularVelocityX;
            this.angularVelocityY = data.m_angularVelocityY;
            this.angularVelocityZ = data.m_angularVelocityZ;
            this.angularAccelerationX = data.m_angularAccelerationX;
            this.angularAccelerationY = data.m_angularAccelerationY;
            this.angularAccelerationZ = data.m_angularAccelerationZ;
            this.frontWheelsAngle = data.m_frontWheelsAngle;
            this.RPMs = F12019NewTelemetryProvider.lastRPMs;
        }
    }

    class ArrayConverter
    {
        public static dynamic ConvVal<T>(byte[] rawData, int idx, out int len)
        {
            if (typeof(T) == typeof(float))
            {
                len = 4;
                return BitConverter.ToSingle(rawData, idx);
            }
            else if (typeof(T) == typeof(UInt16))
            {
                len = 2;
                return BitConverter.ToUInt16(rawData, idx);
            }
            else
            {
                len = 1;
                return rawData[idx];
            }
        }

        public static T[] Conv4Array<T>(byte[] rawData, int idx)
        {
            int len = 0;

            T[] a = new T[4];

            for (int i = 0; i < 4; i++)
            {
                a[i] = (T)ConvVal<T>(rawData, i * len + idx, out len);
            }
            return a;
        }
    }
}
// The motion data package of F1-2018/2019. It is the same format for both game versions.
// Consists of two parts, motion data for all 20 cars (CarMotionData) and addtl. data for the player car in PacketMotionData.
// The player's data is in car 0 in the CarMotionData array (containing all 20 cars in case of a race).

using System;

namespace F12019New
{
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
            // We unpack only car 0 because it is the only one we need.
            this.m_carMotionData[0] = new CarMotionData(rawData, idx); idx += 20 * this.m_carMotionData[0].sz;
            // Comment out the above and uncomment the below if you need all cars
            //for (int i = 0; i < 20; i++)
            //{
            //    this.m_carMotionData[i] = new CarMotionData(rawData, idx); idx += this.m_carMotionData[i].sz;
            //}
            this.m_suspensionPosition = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);
            this.m_suspensionVelocity = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);
            this.m_suspensionAcceleration = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);
            this.m_wheelSpeed = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);
            this.m_wheelSlip = DataConverter.ConvArray<float>(rawData, idx, 4, BitConverter.ToSingle); idx += 4 * sizeof(float);
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
}

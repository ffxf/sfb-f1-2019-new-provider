// The telemetry data we export to SimFeedback.
// It is a collection from parts of the game's motion and telemetry data plus some values we are deriving
// from baseline telemtry keys in F12019NewTelemetryInfo.cs

namespace F12019New
{
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
}
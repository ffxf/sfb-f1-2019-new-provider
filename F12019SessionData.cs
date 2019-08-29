// The session data package of F1-2018/2019. It is the same format for both game versions.

using System;

namespace F12019New
{
    class MarshalZone
    {
        public int sz;

        public float m_zoneStart { get; set; }   // Fraction (0..1) of way through the lap the marshal zone starts
        public byte m_zoneFlag { get; set; }    // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red

        public MarshalZone(byte[] rawData, int idx)
        {
            int start_idx = idx;

            this.m_zoneStart = BitConverter.ToSingle(rawData, idx); idx += sizeof(float);
            this.m_zoneFlag = rawData[idx]; idx += sizeof(byte);
            
            this.sz = idx - start_idx;
        }
    }

    class SessionData
    {
        public PacketHeader m_header;                      // Header
        public byte m_weather { get; set; }                // Weather - 0 = clear, 1 = light cloud, 2 = overcast
                                                           // 3 = light rain, 4 = heavy rain, 5 = storm
        public sbyte m_trackTemperature { get; set; }      // Track temp. in degrees celsius
        public sbyte m_airTemperature { get; set; }        // Air temp. in degrees celsius
        public byte m_totalLaps { get; set; }              // Total number of laps in this race
        public UInt16 m_trackLength { get; set; }          // Track length in metres
        public byte m_sessionType { get; set; }            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                                                           // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                                                           // 10 = R, 11 = R2, 12 = Time Trial
        public sbyte m_trackId { get; set; }               // -1 for unknown, 0-21 for tracks, see appendix
        public byte m_formula { get; set; }                // Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2,
                                                           // 3 = F1 Generic
        public UInt16 m_sessionTimeLeft { get; set; }      // Time left in session in seconds
        public UInt16 m_sessionDuration { get; set; }      // Session duration in seconds
        public byte m_pitSpeedLimit { get; set; }          // Pit speed limit in kilometres per hour
        public byte m_gamePaused { get; set; }             // Whether the game is paused
        public byte m_isSpectating { get; set; }           // Whether the player is spectating
        public byte m_spectatorCarIndex { get; set; }      // Index of the car being spectated
        public byte m_sliProNativeSupport { get; set; }    // SLI Pro support, 0 = inactive, 1 = active
        public byte m_numMarshalZones { get; set; }        // Number of marshal zones to follow
        public MarshalZone[] m_marshalZones { get; set; }  // List of marshal zones – max 21
        public byte m_safetyCarStatus { get; set; }        // 0 = no safety car, 1 = full safety car
                                                           // 2 = virtual safety car
        public byte m_networkGame { get; set; }            // 0 = offline, 1 = online

        public SessionData(byte[] rawData)
        {
            int idx = 0;

            this.m_header = new PacketHeader(rawData, idx); idx += this.m_header.sz;
            this.m_weather = rawData[idx]; idx += sizeof(byte);
            this.m_trackTemperature = (sbyte) rawData[idx]; idx += sizeof(byte);
            this.m_airTemperature = (sbyte) rawData[idx]; idx += sizeof(byte);
            this.m_totalLaps = rawData[idx]; idx += sizeof(byte);
            this.m_trackLength = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_sessionType = rawData[idx]; idx += sizeof(byte);
            this.m_trackId = (sbyte)rawData[idx]; idx += sizeof(byte);
            this.m_formula = rawData[idx]; idx += sizeof(byte);
            this.m_sessionTimeLeft = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_sessionDuration = BitConverter.ToUInt16(rawData, idx); idx += sizeof(UInt16);
            this.m_pitSpeedLimit = rawData[idx]; idx += sizeof(byte);
            this.m_gamePaused = rawData[idx]; idx += sizeof(byte);
            this.m_isSpectating = rawData[idx]; idx += sizeof(byte);
            this.m_spectatorCarIndex = rawData[idx]; idx += sizeof(byte);
            this.m_sliProNativeSupport = rawData[idx]; idx += sizeof(byte);
            this.m_numMarshalZones = rawData[idx]; idx += sizeof(byte);
            this.m_marshalZones = new MarshalZone[21];
            for (int i = 0; i < 21; i++)
            {
                this.m_marshalZones[i] = new MarshalZone(rawData, idx); idx += this.m_marshalZones[i].sz;
            }
            this.m_safetyCarStatus = rawData[idx]; idx += sizeof(byte);
            this.m_networkGame = rawData[idx]; idx += sizeof(byte);
        }
    }
}
// The packet header being at the front of all F1-2018/2019 new format UDP packets
// Note that there is a difference: F1-2019 contains a major and minor version, F1-2018 does not
// In case of F1-2018 we just set these to zero and we take less data out of the byte array with the raw data
// coming from the game.
// We can which game it is by the length of the incoming byte array.

using System;

namespace F12019New
{
    class PacketHeader
    {
        public int sz;

        public UInt16 m_packetFormat { get; set; }         // 2019 or 2018
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
}
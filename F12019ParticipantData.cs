// The participant data package of F1-2018/2019.
// Note that there is a difference: F1-2019 contains a setting whether the telemtry is public, F1-2018 does not
// In case of F1-2018 we just set it to 1 and we take less data out of the byte array with the raw data
// coming from the game.
// We can determine which game it is by the length of the incoming byte array.

using System;

namespace F12019New
{
    class PacketsParticipantData
    {
        public int sz;

        public byte m_aiControlled { get; set; }   // Whether the vehicle is AI (1) or Human (0) controlled
        public byte m_driverId { get; set; }       // Driver id - see appendix
        public byte m_teamId { get; set; }         // Team id - see appendix
        public byte m_raceNumber { get; set; }     // Race number of the car
        public byte m_nationality { get; set; }    // Nationality of the driver
        public char[] m_name { get; set; }         // 48 chars maxName of participant in UTF-8 format – null terminated
                                                   // Will be truncated with … (U+2026) if too long
        byte m_yourTelemetry { get; set; }         // The player's UDP setting, 0 = restricted, 1 = public


        public PacketsParticipantData(byte[] rawData, int idx)
        {
            int start_idx = idx;

            this.m_aiControlled = rawData[idx]; idx += sizeof(byte);
            this.m_driverId = rawData[idx]; idx += sizeof(byte);
            this.m_teamId = rawData[idx]; idx += sizeof(byte);
            this.m_raceNumber = rawData[idx]; idx += sizeof(byte);
            this.m_nationality = rawData[idx]; idx += sizeof(byte);
            this.m_name = DataConverter.ConvArray<char>(rawData, idx, 48, (rd, i) => { return (char) rd[i]; }); idx += 48 * sizeof(char);

            this.m_yourTelemetry = 1;  // F1-2018 default
            if (rawData.Length == F12019NewTelemetryProvider.bytesInParticipantPacket2019)
            {
                this.m_yourTelemetry = rawData[idx]; idx += sizeof(byte);
            }

            this.sz = idx - start_idx;
        }
    }

    class ParticpantData
    {
        public PacketHeader m_header;                        // Header
        public byte m_numActiveCars { get; set; }            // Number of active cars in the data – should match number of
                                                             // cars on HUD
        public PacketsParticipantData[] m_participants { get; set; } // Data for all cars on track
        public int humanDriver { get; set; }                 // set this to the one that is human controlled
                                                             // ==> we do not support multiplayer


        public ParticpantData(byte[] rawData)
        {
            int idx = 0;
            this.m_header = new PacketHeader(rawData, idx); idx += this.m_header.sz;
            this.m_numActiveCars = rawData[idx]; idx += sizeof(byte);
            this.m_participants = new PacketsParticipantData[20];
            for (int i = 0; i < 20; i++)
            {
                this.m_participants[i] = new PacketsParticipantData(rawData, idx); idx += this.m_participants[i].sz;
                if (this.m_participants[i].m_aiControlled == 0)
                {
                    this.humanDriver = i;
                }
            }
        }
    }
}

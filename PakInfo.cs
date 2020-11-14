using System;
using System.Text;

namespace UE_Pak_4_22
{
    public class PakInfo
    {
        public PakFile Pak;
        // size 16
        public byte[] EncryptionKeyGuid;
        public bool EncryptedIndex;
        public UInt32 Magic;
        public Int32 Version;
        public Int64 IndexOffset;
        public Int64 IndexSize;
        // size 20
        public byte[] IndexHash;
        // size 32 * 4
        public string[] CompressionMethods;

        public PakInfo(UE_Reader reader, PakFile pak)
        {
            Pak = pak;
            EncryptionKeyGuid = reader.ReadBytes(16);
            EncryptedIndex = reader.ReadByte() > 0; // need UE Viewer source reference
            Magic = reader.ReadUInt32();
            Version = reader.ReadInt32();
            IndexOffset = reader.ReadInt64();
            IndexSize = reader.ReadInt64();
            IndexHash = reader.ReadBytes(20);
            CompressionMethods = new string[4];
            for (int i = 0; i < CompressionMethods.Length; i++)
            {
                CompressionMethods[i] = GetString(reader.ReadBytes(32));
            }
        }

        static Encoding Enc = Encoding.ASCII;

        static string GetString(byte[] value)
        {
            int i = value.Length - 1;
            for (; i > -1; i--)
            {
                if (value[i] != 0)
                {
                    break;
                }
                else
                {
                    if (i == 0) // end
                    {
                        i = -1;
                        break;
                    }
                }
            }
            if (i == -1)
            {
                return "";
            }
            return Enc.GetString(value, 0, i + 1);
        }
    }
}

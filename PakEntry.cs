using System;

namespace UE_Pak_4_22
{
    public class PakEntry
    {
        public PakIndex Index;
        //
        public string Filename;
        public Int64 Position;
        public Int64 Size;
        public Int64 UncompressedSize;
        public byte CompressionMethodIndex;
        public byte[] Hash;
        public Int32 PakCompressedBlockCount;
        public PakCompressedBlock[] Blocks;
        public bool Encrypted;
        public Int32 CompressionBlockSize;
        //
        public long HeaderSize;

        public string Compression
        {
            get
            {
                if (CompressionMethodIndex == 0)
                {
                    return CompressionMethod.None;
                }
                else
                {
                    return Index.Pak.Info.CompressionMethods[CompressionMethodIndex - 1].ToLower();
                }
            }
        }

        public PakEntry(UE_Reader reader, PakIndex index)
        {
            Index = index;
            Filename = reader.ReadString();
            long start_position = reader.BaseStream.Position;
            Position = reader.ReadInt64();
            if (Position < 0)
            {
                reader.BaseStream.Position += 0x2A;
                HeaderSize = reader.BaseStream.Position - start_position;
                return;
            }
            Size = reader.ReadInt64();
            UncompressedSize = reader.ReadInt64();
            CompressionMethodIndex = reader.ReadByte();
            Hash = reader.ReadBytes(20);
            PakCompressedBlockCount = reader.ReadInt32();
            Blocks = new PakCompressedBlock[PakCompressedBlockCount];
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new PakCompressedBlock(reader);
            }
            Encrypted = reader.ReadByte() > 0; // need UE Viewer source reference
            if (PakCompressedBlockCount > 0)
            {
                CompressionBlockSize = reader.ReadInt32();
            }
            HeaderSize = reader.BaseStream.Position - start_position;
        }
    }
}

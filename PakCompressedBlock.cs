using System;

namespace UE_Pak_4_22
{
   public class PakCompressedBlock
    {
        public Int64 CompressedStart;
        public Int64 CompressedEnd;

        public PakCompressedBlock(UE_Reader reader)
        {
            CompressedStart = reader.ReadInt64();
            CompressedEnd = reader.ReadInt64();
        }
    }
}

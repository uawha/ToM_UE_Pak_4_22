using Newtonsoft.Json;
using System.IO;

namespace UE_Pak_4_22
{
    public class PakFile
    {
        public PakInfo Info;
        public PakIndex Index;

        public PakFile(UE_Reader reader)
        {
            reader.BaseStream.Position = reader.BaseStream.Length - 189;
            Info = new PakInfo(reader, this);
            reader.BaseStream.Position = Info.IndexOffset;
            Index = new PakIndex(reader, this);
        }

        public static PakFile ReadFromFile(string path)
        {
            PakFile pak;
            using (var fs = File.OpenRead(path))
            using (var reader = new UE_Reader(fs, true))
            {
                pak = new PakFile(reader);
            }
            return pak;
        }
    }
}


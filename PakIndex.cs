using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;

namespace UE_Pak_4_22
{
    public class PakIndex
    {
        public static class Naming
        {
            public const string MountPoint = "mount_point";
            public const string PakEntryCount = "entry_count";
            public const string Listing = "listing";
        }

        public PakFile Pak;
        public string MountPoint;
        public Int32 PakEntryCount;
        public PakEntry[] Entries;

        public PakIndex(UE_Reader reader, PakFile pak)
        {
            Pak = pak;
            MountPoint = reader.ReadString();
            PakEntryCount = reader.ReadInt32();
            Entries = new PakEntry[PakEntryCount];
            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i] = new PakEntry(reader, this);
            }
        }

        Listing.DirectoryValue DirectoryValue;
        void CheckDV()
        {
            if (DirectoryValue == null)
            {
                DirectoryValue = new Listing.DirectoryValue();
                foreach (var entry in Entries)
                {
                    DirectoryValue.Feed(entry);
                }
            }
        }

        public void WriteJsonListing(JsonWriter writer)
        {
            CheckDV();
            writer.WriteStartObject();
            writer.WritePropertyName(Naming.MountPoint);
            writer.WriteValue(MountPoint);
            writer.WritePropertyName(Naming.PakEntryCount);
            writer.WriteValue(PakEntryCount);
            writer.WritePropertyName(Naming.Listing);
            DirectoryValue.WriteJson(writer);
            writer.WriteEndObject();
        }

        public void WriteTextListing(TextWriter writer)
        {
            CheckDV();
            writer.WriteLine(MountPoint);
            writer.WriteLine(PakEntryCount);
            writer.WriteLine();
            DirectoryValue.WriteText(writer);
        }
    }

    public class PakFile_Listing_JsonConverter : JsonConverter<PakFile>
    {
        public override bool CanRead => false;
        public override PakFile ReadJson(JsonReader reader, Type objectType, PakFile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override void WriteJson(JsonWriter writer, PakFile value, JsonSerializer serializer)
        {
            value.Index.WriteJsonListing(writer);
        }
    }
}

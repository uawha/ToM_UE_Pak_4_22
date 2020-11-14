using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace UE_Pak_4_22
{
    class Listing
    {
        /// <summary>
        /// At top level, a DirectoryValue is the content of <see cref="PakIndex.MountPoint"/>
        /// </summary>
        public class DirectoryValue
        {
            public Dictionary<string, DirectoryValue> Directories;
            public Dictionary<string, PakEntry> Files;

            public DirectoryValue()
            {
                Directories = new Dictionary<string, DirectoryValue>(StringComparer.OrdinalIgnoreCase);
                Files = new Dictionary<string, PakEntry>(StringComparer.OrdinalIgnoreCase);
            }

            public void Feed(PakEntry entry, string pathOverride = null)
            {
                string path = pathOverride ?? entry.Filename;
                int index_slash = path.IndexOf('/');
                if (index_slash == 0)
                {
                    throw new Exception($"Empty directory name: {path}");
                }
                else if (index_slash > 0)
                {
                    string directory_name = path.Substring(0, index_slash);
                    if (!Directories.TryGetValue(directory_name, out DirectoryValue d_value))
                    {
                        d_value = new DirectoryValue();
                        Directories.Add(directory_name, d_value);
                    }
                    d_value.Feed(entry, path.Substring(index_slash + 1));
                }
                else
                {
                    string file_name = path;
                    if (Files.ContainsKey(file_name))
                    {
                        throw new Exception($"File already exist: {entry.Filename}");
                    }
                    Files.Add(file_name, entry);
                }
            }

            static IFormatProvider US_NumFormat = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            static string GetFileInfo(PakEntry entry)
            {
                PakInfo info = entry.Index.Pak.Info;
                var sb = new StringBuilder();
                sb.Append($"header_size:{entry.HeaderSize}");
                if (entry.Position < 0)
                {
                    sb.Append(" not_present");
                    return sb.ToString();
                }
                // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
                string str_size = String.Format(US_NumFormat, "{0:N0}", entry.UncompressedSize);
                sb.Append($" size:{str_size}");
                if (entry.Encrypted)
                {
                    sb.Append(" encrypted");
                }
                sb.Append($" compression:{entry.Compression}");
                sb.Append($" c_block_count:{entry.PakCompressedBlockCount}");
                return sb.ToString();
            }

            public void WriteJson(JsonWriter writer)
            {
                writer.WriteStartObject();
                var dir_name_list = new List<string>(Directories.Keys);
                dir_name_list.Sort();
                foreach (var dir_name in dir_name_list)
                {
                    writer.WritePropertyName(dir_name);
                    Directories[dir_name].WriteJson(writer);
                }
                var file_name_list = new List<string>(Files.Keys);
                file_name_list.Sort();
                foreach (var file_name in file_name_list)
                {
                    writer.WritePropertyName(file_name);
                    writer.WriteValue(GetFileInfo(Files[file_name]));
                }
                writer.WriteEndObject();
            }

            public void WriteText(TextWriter writer)
            {
                var file_name_list = new List<string>(Files.Keys);
                file_name_list.Sort();
                foreach (var file_name in file_name_list)
                {
                    writer.WriteLine(Files[file_name].Filename);
                    writer.WriteLine(GetFileInfo(Files[file_name]));
                    writer.WriteLine();
                }
                var dir_name_list = new List<string>(Directories.Keys);
                dir_name_list.Sort();
                foreach (var dir_name in dir_name_list)
                {
                    Directories[dir_name].WriteText(writer);
                }
            }
        }
    }
}

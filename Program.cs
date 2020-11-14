using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace UE_Pak_4_22
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (var f in Directory.EnumerateFiles(args[0], "*.pak"))
            //{
            //    CreateListing(f);
            //}
            //string DLC = @"G:\Games\Trials of Mana\Trials of Mana\Content\Paks\DLC01Trials of Mana-WindowsNoEditor.pak";
            //string DLC_De_Dir = @"E:\ToM_Decompress\DLC";
            //DecompressPak(DLC, DLC_De_Dir);

            string MAIN_0 = @"G:\Games\Trials of Mana\Trials of Mana\Content\Paks\Trials of Mana-WindowsNoEditor_0_P.pak";
            string MAIN_0_De_Dir = @"E:\ToM_Decompress\Main_0";
            DecompressPak(MAIN_0, MAIN_0_De_Dir);

            //string MAIN = @"G:\Games\Trials of Mana\Trials of Mana\Content\Paks\Trials of Mana-WindowsNoEditor.pak";
            //string MAIN_De_Dir = @"E:\ToM_Decompress\Main";
            //DecompressPak(MAIN, MAIN_De_Dir);

            Console.WriteLine("Done.");
        }

        const string ListingDirectoryName = "listing";

        static void CreateListing(string pak_file)
        {
            // path prepare
            pak_file = Path.GetFullPath(pak_file);
            Console.WriteLine(pak_file);
            string dir = Path.GetDirectoryName(pak_file);
            string listing_dir = Path.Combine(dir, ListingDirectoryName);
            Directory.CreateDirectory(listing_dir);
            string pak_file_listing = Path.Combine(listing_dir, Path.GetFileName(pak_file) + ".json");
            string txt_file_listing = Path.Combine(listing_dir, Path.GetFileName(pak_file) + ".txt");
            //
            // read pak info
            PakFile pak = PakFile.ReadFromFile(pak_file);
            Console.WriteLine($"Mount point: {pak.Index.MountPoint}");
            Console.WriteLine($"File count: {pak.Index.PakEntryCount}");
            //
            // do se
            var json_se = new JsonSerializer();
            json_se.Converters.Add(new PakFile_Listing_JsonConverter());
            json_se.NullValueHandling = NullValueHandling.Include;
            json_se.Formatting = Formatting.Indented;
            //
            using (var fs = File.Create(pak_file_listing))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false, true)))
            using (var jw = new JsonTextWriter(sw))
            {
                json_se.Serialize(jw, pak);
            }
            Console.WriteLine($"Written to: {pak_file_listing}");
            //
            using (var fs = File.Create(txt_file_listing))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false, true)))
            {
                pak.Index.WriteTextListing(sw);
            }
            Console.WriteLine($"Written to: {txt_file_listing}");
            Console.WriteLine();
        }

        static void DecompressPak(string pak_file, string out_directory)
        {
            Directory.CreateDirectory(out_directory);
            PakFile pak = PakFile.ReadFromFile(pak_file);
            using (var fs = File.OpenRead(pak_file))
            using (var reader = new UE_Reader(fs, true))
            {
                foreach (var entry in pak.Index.Entries)
                {
                    Console.WriteLine($"Processing {entry.Filename}");
                    if (entry.Encrypted || entry.Position < 0)
                    {
                        Console.WriteLine("Encrypted or not present, pass.");
                        continue;
                    }
                    string out_p = Path.Combine(out_directory, entry.Filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(out_p));
                    using (var fs_w = File.Create(out_p))
                    {
                        Decompress.懒得起名(fs_w, reader, entry);
                    }
                }
            }
        }
    }

}

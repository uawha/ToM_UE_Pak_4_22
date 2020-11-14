using System;
using System.IO;
using Ionic.Zlib;

namespace UE_Pak_4_22
{
    class Decompress
    {
        public static void 懒得起名(Stream output, UE_Reader reader, PakEntry entry)
        {
            if (entry.Encrypted || entry.Position < 0)
            {
                throw new Exception("把这个判断句复制到 caller");
            }
            if (entry.Compression == CompressionMethod.None)
            {
                reader.BaseStream.Position = entry.Position + entry.HeaderSize;
                var _R = reader.ReadBytes((int)(entry.UncompressedSize));
                output.Write(_R, 0, _R.Length);
            }
            else if (entry.Compression == CompressionMethod.Zlib)
            {
                for (int i = 0; i < entry.Blocks.Length; i++)
                {
                    byte[] cnt = ReadBlock(reader, entry, entry.Blocks[i]);
                    var _R = ZlibStream.UncompressBuffer(cnt);
                    output.Write(_R, 0, _R.Length);
                }
            }
            else
            {
                throw new NotImplementedException($"Compression Method {entry.Compression} is not implemented.");
            }
        }

        static byte[] ReadBlock(UE_Reader reader, PakEntry entry, PakCompressedBlock b_info)
        {
            reader.BaseStream.Position = entry.Position + b_info.CompressedStart;
            long length = b_info.CompressedEnd - b_info.CompressedStart;
            return reader.ReadBytes((int)length);
        }
    }
}

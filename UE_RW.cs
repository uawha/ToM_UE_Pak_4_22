using System;
using System.IO;
using System.Text;

namespace UE_Pak_4_22
{
    public class UE_Reader : BinaryReader
    {
        static readonly Encoding Utf8Enc = new UTF8Encoding(false, true);

        public UE_Reader(Stream input, bool leaveOpen) : base(input, Utf8Enc, leaveOpen) { }

        public new string ReadString()
        {
            if (Position超出范围)
            {
                throw new EndOfStreamException();
            }

            long position = BaseStream.Position;

            uint u_length = base.ReadUInt32();
            if (u_length > Int32.MaxValue)
            {
                throw new Exception($"Offset {position:X8}: this is unlikely a string. Its length is too large: {u_length}.");
            }
            int length = (int)u_length;
            if (length >= 0x1000)
            {
                throw new Exception($"Offset {position:X8}: this is unlikely a string. Its length is too large: {u_length}.");
            }
            if (length == 0) return null;
            if (length == 1) return "";
            var valueBytes = base.ReadBytes(length);
            if (valueBytes[length - 1] != 0)
            {
                throw new Exception($"Offset {position:X8}: this is unlikely a string. It is not 0x00 terminated.");
            }
            if (valueBytes[length - 2] == 0)
            {
                throw new Exception($"Offset {position:X8}: this is unlikely a string. It is 0x00 terminated but before that is also a 0x00.");
            }
            return Utf8Enc.GetString(valueBytes, 0, valueBytes.Length - 1);
        }

        public bool Position超出范围 => base.BaseStream.Position >= base.BaseStream.Length;
    }

    public class UE_Writer : BinaryWriter
    {
        static readonly Encoding Utf8Enc = new UTF8Encoding(false, true);

        public UE_Writer(Stream input, bool leaveOpen) : base(input, Utf8Enc, leaveOpen) { }

        public new void Write(string value)
        {
            if (value == null)
            {
                base.Write(0);
                return;
            }
            var valueBytes = Utf8Enc.GetBytes(value);
            base.Write(valueBytes.Length + 1); // int or uint
            if (valueBytes.Length > 0)
            {
                base.Write(valueBytes);
            }
            base.Write((byte)0);
        }
    }
}

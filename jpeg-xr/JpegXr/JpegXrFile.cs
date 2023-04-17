using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Org.OpenOrbit.Libraries.JpegXr.Exceptions;
using Org.OpenOrbit.Libraries.JpegXr.Header.ImageFileDirectory;
using Org.OpenOrbit.Libraries.JpegXr.Utils;

namespace Org.OpenOrbit.Libraries.JpegXr
{
    internal class JpegXrFile
    {
        private static uint DecodeHeader(BinaryReader reader)
        {
            var header = (reader.ReadUInt16() << 8) | reader.ReadByte();
            if (header != 0x4949BC)
                throw new InvalidFormatException("JpegXr Header", "0x4949BC", header.Hex());

            var version = reader.ReadByte();
            if (version != 0x1)
                throw new InvalidFormatException("JpegXr version", "1", version);

            var firstIfdOffset = reader.ReadUInt32();
            return firstIfdOffset;
        }


        public static JpegXrFile Decode(Stream stream)
        {
            var reader = new BinaryReader(stream);

            var nextIfdOffset = DecodeHeader(reader);

            var imageFileDirectories = new List<ImageFileDirectory>();
            do
            {
                reader.BaseStream.Seek(nextIfdOffset, SeekOrigin.Begin);
                var ifd = ImageFileDirectory.Decode(reader);
                imageFileDirectories.Add(ifd);
                nextIfdOffset = reader.ReadUInt32();
            } while (nextIfdOffset != 0);

            foreach (var entry in imageFileDirectories.SelectMany(ifd => ifd.Entries))
            {
                Console.WriteLine(entry.Tag + " \t " + entry.ElementType.Size() * entry.Tag.NumElements() +
                                  " Bytes \t => " + entry.ValuesOrOffset);

                if (entry.Tag != FieldTag.PixelFormat) continue;
                
                // save current position and seek to the location of the PixelFormat value
                var readerPos = reader.BaseStream.Position;
                reader.BaseStream.Seek(entry.ValuesOrOffset, SeekOrigin.Begin);
                
                // read the pixelFormat value and seek back to the previous position
                var format = reader.ReadBytes(16).Reverse().ToArray();
                reader.BaseStream.Seek(readerPos, SeekOrigin.Begin);
                
                // compare the values with the expected ones...
                var e2 = 0x24C3DD6F034EFE4Bu;
                var e1 = 0xB1853D77768DC90Au;
                var a1 = BitConverter.ToUInt64(format);
                var a2 = BitConverter.ToUInt64(format,8);
                if (e1 != a1)
                {
                    throw new Exception("e1 != a1");
                }
                if (e2 != a2)
                {
                    throw new Exception("e2 != a2");
                }
                Console.WriteLine("All good!");
            }

            throw new NotImplementedException("JpegXrFile.Decode");
            return new JpegXrFile();
        }
    }
}

using System;
using System.IO;
using Org.OpenOrbit.Libraries.JpegXr.Exceptions;
using Org.OpenOrbit.Libraries.JpegXr.Utils;

namespace Org.OpenOrbit.Libraries.JpegXr.Header.ImageFileDirectory
{
    internal class ImageFileDirectoryEntry
    {
        public FieldTag Tag { get; private set; }
        public ElementType ElementType { get; private set; }
        public uint NumElements { get; private set; }
        public uint ValuesOrOffset { get; private set; }

        public static ImageFileDirectoryEntry Decode(BinaryReader reader)
        {
            var tagNum = reader.ReadUInt16();

            if (!Enum.IsDefined(typeof(FieldTag), (int)tagNum))
            {
                reader.BaseStream.Seek(-2, SeekOrigin.Current);
                throw new InvalidFormatException(
                    "ImageFileDirectoryEntry.Tag at offset " + reader.BaseStream.Position.Hex(),
                    Enum.GetValues(typeof(FieldTag)), tagNum);
            }

            var elementTypeNum = reader.ReadUInt16();

            if (!Enum.IsDefined(typeof(ElementType), (int)elementTypeNum))
            {
                reader.BaseStream.Seek(-2, SeekOrigin.Current);
                throw new InvalidFormatException(
                    "ImageFileDirectoryEntry.ElementType at offset " + reader.BaseStream.Position.Hex(),
                    Enum.GetValues(typeof(ElementType)), elementTypeNum);
            }


            var entry = new ImageFileDirectoryEntry
            {
                Tag = (FieldTag)tagNum,
                ElementType = (ElementType)elementTypeNum,
                NumElements = reader.ReadUInt32(),
                ValuesOrOffset = reader.ReadUInt32()
            };
            return entry;
        }
    }
}
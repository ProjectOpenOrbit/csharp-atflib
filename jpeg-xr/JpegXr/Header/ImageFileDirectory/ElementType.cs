using System;

namespace Org.OpenOrbit.Libraries.JpegXr.Header.ImageFileDirectory
{
    internal static class ElementTypeExtensions
    {
        public static int Size(this ElementType type)
        {
            return type switch
            {
                ElementType.Byte => 1,
                ElementType.Utf8 => 1,
                ElementType.Ushort => 2,
                ElementType.Ulong => 4,
                ElementType.Urational => 8,
                ElementType.Sbyte => 1,
                ElementType.Undefined => 1,
                ElementType.Sshort => 2,
                ElementType.Slong => 4,
                ElementType.Srational => 8,
                ElementType.Float => 4,
                ElementType.Double => 8,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    internal enum ElementType
    {
        Byte = 1,
        Utf8 = 2,
        Ushort = 3,
        Ulong = 4,
        Urational = 5,
        Sbyte = 6,
        Undefined = 7,
        Sshort = 8,
        Slong = 9,
        Srational = 10,
        Float = 11,
        Double = 12
    }
}
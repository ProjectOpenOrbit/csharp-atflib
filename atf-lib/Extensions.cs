namespace Org.OpenOrbit.Libraries.Atf;

public static class Extensions
{
    public static uint ReadUInt32Be(this BinaryReader reader)
    {
        return BitConverter.ToUInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
    }
}
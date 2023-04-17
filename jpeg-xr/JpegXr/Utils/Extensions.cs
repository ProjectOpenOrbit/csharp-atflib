namespace Org.OpenOrbit.Libraries.JpegXr.Utils
{
    public static class Extensions
    {
        public static string Hex(this int num)
        {
            return "0x" + num.ToString("X");
        }
        public static string Hex(this long num)
        {
            return "0x" + num.ToString("X");
        }
    }
}
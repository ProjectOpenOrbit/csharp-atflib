using System.IO;

namespace Org.OpenOrbit.Libraries.JpegXr
{
    public static class JpegXrDecoder
    {
        public static void Decode(Stream stream)
        {
            JpegXrFile.Decode(stream);
        }
    }
}
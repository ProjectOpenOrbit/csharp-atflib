using System.IO.Compression;
using SevenZip.Compression.LZMA;

namespace Atf
{
    public static class CompressionUtil
    {
        
        public static byte[] DecompressLzma(byte[] compressedData)
        {
            try
            {
                MemoryStream outStr = new MemoryStream();
                var decoder = new Decoder();
                var compressedStream = new BinaryReader(new MemoryStream(compressedData));
                
                decoder.SetDecoderProperties(compressedStream.ReadBytes(5));
                decoder.Code(compressedStream.BaseStream, outStr, compressedData.Length-5, Int32.MaxValue, null);
                return outStr.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            
        }

        public static BinaryReader DecompressZlib(Stream compressedStream)
        {
            var deflater = new DeflateStream(compressedStream,CompressionMode.Decompress);
            return new BinaryReader(deflater);
        }
    }
}
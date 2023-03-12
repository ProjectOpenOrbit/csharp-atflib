using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;


namespace Atf
{
    public class ATFReader
    {
        private readonly ATFFormat _atfFormat;
        private readonly int _height;
        private readonly int _version;
        private readonly int _width;
        private bool _cubemap;
        private int _mipCount;

        public ATFReader(Stream data)
        {
            Stream atfStream = new MemoryStream();
            var b = new byte[2];
            var n = data.Read(b, 0, 2);
            data.Position = 0;
            if (n == 2 && b[0] == 0x1f && b[1] == 0x8b) // atf.gz without extension
                using (var stream = new GZipStream(data, CompressionMode.Decompress))
                {
                    stream.CopyTo(atfStream);
                }
            else
                atfStream = data;

            atfStream.Position = 0;
            if (atfStream.Length <= 20) throw new Exception("Too small to be a valid ATF data");

            using var reader = new BinaryReader(atfStream, Encoding.UTF8);
            if (new string(reader.ReadChars(3)) != "ATF") throw new Exception("No ATF signature found");

            b = reader.ReadBytes(4);
            uint length = 0;
            if (b[3] != 0xff)
            {
                _version = 0;
                length = ((uint)b[0] << 16) + ((uint)b[1] << 8) + b[2];
                reader.BaseStream.Position = reader.BaseStream.Position - 1;
            }
            else
            {
                _version = reader.ReadByte();
                length = BitConverter.IsLittleEndian
                    ? BitConverter.ToUInt32(reader.ReadBytes(4).Reverse().ToArray(), 0)
                    : reader.ReadUInt32();
            }

            if (length + reader.BaseStream.Position != reader.BaseStream.Length)
                throw new Exception("ATF length mismatch");
            var check = reader.ReadByte();
            _cubemap = check >> 7 == 1;
            _atfFormat = (ATFFormat)(check & 0x7f);
            _width = 1 << reader.ReadByte();
            _height = 1 << reader.ReadByte();
            _mipCount = reader.ReadByte();
            //currently we are interested only in highest quality texture and don't bother about cubemaps, so extract only first one.
            if (_version == 0)
            {
                b = reader.ReadBytes(3);
                length = ((uint)b[0] << 16) + ((uint)b[1] << 8) + b[2];
            }
            else
            {
                length = BitConverter.IsLittleEndian
                    ? BitConverter.ToUInt32(reader.ReadBytes(4).Reverse().ToArray(), 0)
                    : reader.ReadUInt32();
            }

            byte[] image;
            switch (_atfFormat)
            {
                case ATFFormat.RawCompressed:
                    image = DxtUtil.DecompressDxt1(reader.ReadBytes((int)length), _width, _height);
                    break;
                case ATFFormat.RawCompressedAlpha:
                    image = DxtUtil.DecompressDxt5(reader.ReadBytes((int)length), _width, _height);
                    break;
                case ATFFormat.Compressed:
                    var dxt1Data = reader.ReadBytes((int)length);
                    
                    //File.WriteAllBytes("imagedata.bin",dxt1Data);
                    var dxt1DataDecompressed = CompressionUtil.DecompressZlib(reader.BaseStream);

                    image = null;
                    break;

                case ATFFormat.Rgb888:
                case ATFFormat.Rgba8888:
                case ATFFormat.CompressedAlpha:
                case ATFFormat.CompressedLossy:
                case ATFFormat.AlphaCompressedLossy:
                default:
                    throw new Exception("Only ATF without lzma or jpeg compression supported now. Format " +
                                        _atfFormat + " not yet implemented");
            }

            Bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
            var pixels = Bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(image, 0, pixels.Scan0, image.Length);
            Bitmap.UnlockBits(pixels);
        }

        public Bitmap Bitmap { get; }
    }
}
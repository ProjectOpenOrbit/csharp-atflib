﻿using System.IO.Compression;
using System.Text;
using Atf;
using Org.OpenOrbit.Libraries.JpegXr;

namespace Org.OpenOrbit.Libraries.Atf;

public enum AtfFormat
{
    ATFRGB888 = 0, //JXRC_FMT_24bppRGB
    ATFRGBA8888 = 1, //JXRC_FMT_32bppBGRA

    ATFCOMPRESSED =
        2, //LZMA compressed DXT1 data + JXRC_FMT_16bppBGR565 + LZMA compressed PVRTC top data + LZMA compressed PVRTC bottom data + JXRC_FMT_16bppBGR555 + LZMA compressed ETC1 top data + LZMA compressed ETC1 bottom data + JXRC_FMT_16bppBGR555 + LZMA compressed ETC2Rgb top data + LZMA compressed ETC2Rgb bottom data + JXRC_FMT_24bppBGR
    ATFRAWCOMPRESSED = 3, //RAW DXT1 data + RAW PVRTC data + RAW ETC1 data + RAW ETC2Rgb data

    ATFCOMPRESSEDALPHA =
        4, //LZMA compressed DXT5 data (alpha) + JXRC_FMT_8bppGray (alpha) + LZMA compressed DXT5 data + JXRC_FMT_16bppBGR565 (image) +.....
    ATFRAWCOMPRESSEDALPHA = 5, //RAW DXT5 data + RAW PVRTC data + RAW ETC1 data + RAW ETC2Rgba data
    ATFCOMPRESSEDLOSSY = 0x0c, //LZMA compressed DXT1 data + JXRC_FMT_16bppBGR565 + ....

    ATFALPHACOMPRESSEDLOSSY =
        0x0d //LZMA compressed DXT5 data (alpha) + JXRC_FMT_8bppGray (alpha) + LZMA compressed DXT5 data + JXRC_FMT_16bppBGR565 (image) + ...
}

public class AtfReader
{
    private readonly AtfFormat _atfFormat;
    private readonly int _height;
    private readonly int _version;

    private readonly int _width;
    private bool _cubemap;

    private int _mipCount;
    //  public Bitmap Bitmap = new Bitmap(1, 1);

    public AtfReader(Stream data)
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

        using (var reader = new BinaryReader(atfStream, Encoding.UTF8))
        {
            var str = new string(reader.ReadChars(3));
            if (str != "ATF") throw new Exception("No ATF signature found");

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
                    ? reader.ReadUInt32Be()
                    : reader.ReadUInt32();
            }

            if (length + reader.BaseStream.Position != reader.BaseStream.Length)
                throw new Exception("ATF length mismatch");
            var check = reader.ReadByte();
            _cubemap = check >> 7 == 1;
            _atfFormat = (AtfFormat)(check & 0x7f);
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
                    ? reader.ReadUInt32Be()
                    : reader.ReadUInt32();
            }

            byte[] image;
            switch (_atfFormat)
            {
                case AtfFormat.ATFRAWCOMPRESSED:
                    image = DxtUtil.DecompressDxt1(reader.ReadBytes((int)length), _width, _height);
                    break;
                case AtfFormat.ATFRAWCOMPRESSEDALPHA:
                    image = DxtUtil.DecompressDxt5(reader.ReadBytes((int)length), _width, _height);
                    break;
                case AtfFormat.ATFCOMPRESSED:
                    // var dxt1DataLen = BitConverter.ToUInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);

                    var dxt1Data = reader.ReadBytes((int)length);
                    dxt1Data = CompressionUtil.DecompressLzma(dxt1Data);

                    var dxt1ImageDataLength = reader.ReadUInt32Be();
                    var dxt1ImageData = reader.ReadBytes((int)dxt1ImageDataLength);

                    JpegXrDecoder.Decode(new MemoryStream(dxt1ImageData));

                    image = DxtUtil.DecompressDxt1Sep(dxt1Data, dxt1ImageData, _width, _height);
                    
                    break;
                default:
                    throw new Exception("Only ATF without lzma or jpeg compression supported now. Format " +
                                        _atfFormat + " not yet implemented");
            }

            // Bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
            // var pixels = Bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly,
            //     PixelFormat.Format32bppArgb);
            // Marshal.Copy(image, 0, pixels.Scan0, image.Length);
            // Bitmap.UnlockBits(pixels);
        }
    }
}
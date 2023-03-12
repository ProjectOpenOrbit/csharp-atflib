namespace Atf;

public enum ATFFormat
{
    /**
     * JXRC_FMT_24bppRGB
     */
    Rgb888 = 0,

    /**
     * JXRC_FMT_32bppBGRA
     */
    Rgba8888 = 1,

    /**
     * LZMA compressed DXT1 data + JXRC_FMT_16bppBGR565 + LZMA compressed PVRTC top data + LZMA compressed PVRTC bottom data + JXRC_FMT_16bppBGR555 + LZMA compressed ETC1 top data + LZMA compressed ETC1 bottom data + JXRC_FMT_16bppBGR555 + LZMA compressed ETC2Rgb top data + LZMA compressed ETC2Rgb bottom data + JXRC_FMT_24bppBGR
     */
    Compressed = 2,

    /**
     * RAW DXT1 data + RAW PVRTC data + RAW ETC1 data + RAW ETC2Rgb data
     */
    RawCompressed = 3,

    /**
     * LZMA compressed DXT5 data (alpha) + JXRC_FMT_8bppGray (alpha) + LZMA compressed DXT5 data + JXRC_FMT_16bppBGR565 (image) +.....
     */
    CompressedAlpha = 4,

    /**
     * RAW DXT5 data + RAW PVRTC data + RAW ETC1 data + RAW ETC2Rgba data
     */
    RawCompressedAlpha = 5,

    /**
     * LZMA compressed DXT1 data + JXRC_FMT_16bppBGR565 + ....
     */
    CompressedLossy = 0x0c,

    /**
     * LZMA compressed DXT5 data (alpha) + JXRC_FMT_8bppGray (alpha) + LZMA compressed DXT5 data + JXRC_FMT_16bppBGR565 (image) + ...
     */
    AlphaCompressedLossy = 0x0d
}
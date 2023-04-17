using ManagedLzma.LZMA;

namespace Org.OpenOrbit.Libraries.Atf;

public static class CompressionUtil
{
    private static byte[] FixLzmaBytes(IReadOnlyList<byte> buffer)
    {
        var fixedLzma = new byte[buffer.Count + 4 + 8]; // 4 bytes at the end (end of stream mark), 8 bytes from pos 5

        // APPEND HEADER
        for (var i = 0; i < 5; i++)
        {
            Console.WriteLine(buffer[i]);
            fixedLzma[i] = buffer[i];
        }

        // APPEND UNCOMPRESSED SIZE INFO (ALL FF BECAUSE THE SIZE IS UNKNOWN)
        for (var i = 0; i < 8; i++) fixedLzma[5 + i] = 0xFF;

        // APPEND BODY
        for (var i = 5; i < buffer.Count; i++) fixedLzma[i + 8] = buffer[i];

        // APPEND EOS TOKEN

        fixedLzma[fixedLzma.Length - 1] = 0xFF;
        fixedLzma[fixedLzma.Length - 2] = 0xFF;
        fixedLzma[fixedLzma.Length - 3] = 0xFF;
        fixedLzma[fixedLzma.Length - 4] = 0xFF;
        return fixedLzma;
    }

    public static byte[] DecompressLzma(IReadOnlyList<byte> buffer)
    {
        var fixedLzma = FixLzmaBytes(buffer);

        var settings = DecoderSettings.ReadFrom(fixedLzma, 0);

        var decoder = new Decoder(settings);

        decoder.Decode(fixedLzma, 13, fixedLzma.Length - 13, 0xFFFFFFF, true);

        var outputBytes = new byte[decoder.AvailableOutputLength];

        decoder.ReadOutputData(outputBytes, 0, decoder.AvailableOutputLength);

        return outputBytes;
    }
}
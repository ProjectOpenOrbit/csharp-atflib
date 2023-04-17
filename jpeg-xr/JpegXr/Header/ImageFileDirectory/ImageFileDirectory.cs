using System.IO;

namespace Org.OpenOrbit.Libraries.JpegXr.Header.ImageFileDirectory
{
    internal class ImageFileDirectory
    {
        public ImageFileDirectoryEntry[] Entries { get; private set; }
        
        
        public static ImageFileDirectory Decode(BinaryReader reader)
        {
            var iNumEntries = reader.ReadUInt16();
            
            var entries = new ImageFileDirectoryEntry[iNumEntries];
            
            for (var i = 0; i < iNumEntries; i++)
            {
                entries[i] = ImageFileDirectoryEntry.Decode(reader);
            }

            return new ImageFileDirectory
            {
                Entries = entries
            };
        }
    }
}
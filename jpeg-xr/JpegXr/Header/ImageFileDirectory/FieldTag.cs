using System;
using System.Collections.Generic;

namespace Org.OpenOrbit.Libraries.JpegXr.Header.ImageFileDirectory
{
    internal static class FieldTagExtensions
    {
        public static readonly List<FieldTag> RequiredTags = new List<FieldTag>()
        {
            FieldTag.PixelFormat,
            FieldTag.ImageWidth,
            FieldTag.ImageHeight,
            FieldTag.ImageOffset,
            FieldTag.ImageByteCount
        };

        public static bool Required(this FieldTag tag)
        {
            return RequiredTags.Contains(tag);
        }

        public static int NumElements(this FieldTag tag)
        {
            return tag switch
            {
                FieldTag.DocumentName => -1,
                FieldTag.ImageDescription => -1,
                FieldTag.EquipmentMake => -1,
                FieldTag.EquipmentModel => -1,
                FieldTag.PageName => -1,
                FieldTag.PageNumber => 2,
                FieldTag.SoftwareNameVersion => -1,
                FieldTag.DateTime => 20,
                FieldTag.ArtistName => -1,
                FieldTag.HostComputer => -1,
                FieldTag.CopyrightNotice => -1,
                FieldTag.ColorSpace => 1,
                FieldTag.PixelFormat => 16,
                FieldTag.SpatialXfrmPrimary => 1,
                FieldTag.ImageType => 1,
                FieldTag.PtmColorInfo => 4,
                FieldTag.ProfileLevelContainer => -1,
                FieldTag.ImageWidth => 1,
                FieldTag.ImageHeight => 1,
                FieldTag.WidthResolution => 1,
                FieldTag.HeightResolution => 1,
                FieldTag.ImageOffset => 1,
                FieldTag.ImageByteCount => 1,
                FieldTag.AlphaOffset => 1,
                FieldTag.AlphaByteCount => 1,
                FieldTag.ImageBandPresence => 1,
                FieldTag.AlphaBandPresence => 1,
                FieldTag.PaddingData => -1,
                _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
            };
        }
    }

    internal enum FieldTag
    {
        DocumentName = 0x010D,
        ImageDescription = 0x010E,
        EquipmentMake = 0x010F,
        EquipmentModel = 0x0110,
        PageName = 0x011D,
        PageNumber = 0x0129,
        SoftwareNameVersion = 0x0131,
        DateTime = 0x0132,
        ArtistName = 0x013B,
        HostComputer = 0x013C,
        CopyrightNotice = 0x8298,
        ColorSpace = 0xA001,
        PixelFormat = 0xBC01,
        SpatialXfrmPrimary = 0xBC02,
        ImageType = 0xBC04,
        PtmColorInfo = 0xBC05,
        ProfileLevelContainer = 0xBC06,
        ImageWidth = 0xBC80,
        ImageHeight = 0xBC81,
        WidthResolution = 0xBC82,
        HeightResolution = 0xBC83,
        ImageOffset = 0xBCC0,
        ImageByteCount = 0xBCC1,
        AlphaOffset = 0xBCC2,
        AlphaByteCount = 0xBCC3,
        ImageBandPresence = 0xBCC4,
        AlphaBandPresence = 0xBCC5,
        PaddingData = 0xEA1C
    }
}
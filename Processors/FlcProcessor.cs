using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using WDViewer.Assets;
using WDViewer.Reader.Flc;

namespace WDViewer.Reader
{
    public class FlcProcessor : IAssetProcessor
    {
        private const ushort FLI_MAGIC_NUMBER = 0xAF11;
        private const ushort FLC_MAGIC_NUMBER = 0xAF12;

        private const ushort FLI_FRAME_MAGIC_NUMBER = 0xF1FA;

        private const ushort FLI_COLOR_256_CHUNK = 4;
        private const ushort FLI_DELTA_CHUNK = 7;
        private const ushort FLI_COLOR_64_CHUNK = 11;
        private const ushort FLI_LC_CHUNK = 12;
        private const ushort FLI_BLACK_CHUNK = 13;
        private const ushort FLI_BRUN_CHUNK = 15;
        private const ushort FLI_COPY_CHUNK = 16;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private unsafe struct FlcHeader
        {
            public UInt32 size;
            public UInt16 type;
            public UInt16 frames; /* Number of frames in first segment */
            public UInt16 width; /* FLIC width in pixels */
            public UInt16 height; /* FLIC height in pixels */
            public UInt16 depth; /* Bits per pixel (usually 8) */
            public UInt16 flags; /* Set to zero or to three */
            public UInt32 speed; /* Delay between frames */
            public UInt16 reserved1; /* Set to zero */
            public UInt32 created; /* Date of FLIC creation (FLC only) */
            public UInt32 creator; /* Serial number or compiler id (FLC only) */
            public UInt32 updated; /* Date of FLIC update (FLC only) */
            public UInt32 updater; /* Serial number (FLC only), see creator */
            public UInt16 aspect_dx; /* Width of square rectangle (FLC only) */
            public UInt16 aspect_dy; /* Height of square rectangle (FLC only) */
            public fixed byte reserved2[38]; /* Set to zero */
            public UInt32 oframe1; /* Offset to frame 1 (FLC only) */

            public UInt32 oframe2; /* Offset to frame 2 (FLC only) */
            public fixed byte reserved3[40]; /* Set to zero */
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private unsafe struct FlcChunkHeader
        {
            public UInt32 size;
            public UInt16 type;
            public UInt16 numSubChunk;
            public fixed byte reserved[8];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FlcSubChunkHeader
        {
            public UInt32 size;
            public UInt16 type;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FlcFrameChunkInfo
        {
            public UInt16 delay;
            public UInt16 reserved;
            public UInt16 width;
            public UInt16 height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FlcPStampChunk
        {
            public UInt16 height;
            public UInt16 width;
            public UInt16 xlate;
        }

        public Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries)
        {
            if (!path.ToLower().EndsWith(".flc"))
            {
                return new Tuple<bool, Asset>(false, null);
            }

            using var stream = new MemoryStream(content);
            using var binReader = new BinaryReader(stream);

            var header = IOHelper.ReadTypeFromReader<FlcHeader>(binReader);
            if (header.type != FLC_MAGIC_NUMBER)
            {
                return new Tuple<bool, Asset>(false, null);
            }

            var flcAsset = new AssetVideo()
            {
                Height = header.height,
                Width = header.width,
                NumFrames = header.frames,
                Delay = header.speed,
                Frames = new Image[header.frames],
                Path = path,
            };

            var frame = new FlcFrame();
            for (var frameIdx = 0; frameIdx < header.frames; ++frameIdx)
            {
                var chunkHeader = IOHelper.ReadTypeFromReader<FlcChunkHeader>(binReader);

                switch (chunkHeader.type)
                {
                    case FLI_FRAME_MAGIC_NUMBER:
                        ReadFrame(binReader, chunkHeader, header, frame);
                        break;
                    default:
                        Console.WriteLine($@"unknown header type {chunkHeader.type}");
                        break;
                }

                if (frame.Pixels is null || frame.ColorMap is null)
                {
                    continue;
                }

                var palette = new AssetPalette()
                {
                    Palette = frame.ColorMap,
                };
                flcAsset.Frames[frameIdx] =
                    ReaderCommon.CreatePaletteImage(header.width, header.height, frame.Pixels, palette, false);
            }

            return new Tuple<bool, Asset>(true, flcAsset);
        }

        private static unsafe void ReadFrame(BinaryReader binReader, FlcChunkHeader flcChunkHeader, FlcHeader videoHeader,
            FlcFrame frame)
        {
            var frameHeader = IOHelper.ByteToType<FlcFrameChunkInfo>(flcChunkHeader.reserved);
            for (var i = 0; i < flcChunkHeader.numSubChunk; ++i)
            {
                ReadChunk(binReader, frame, videoHeader);
            }
        }

        private static void ReadChunk(BinaryReader binReader, FlcFrame frame, FlcHeader videoHeader)
        {
            var chunkHeader = IOHelper.ReadTypeFromReader<FlcSubChunkHeader>(binReader);
            var chunkContent = binReader.ReadBytes((int) (chunkHeader.size - Marshal.SizeOf<FlcSubChunkHeader>()));
            using var stream = new MemoryStream(chunkContent);
            using var chunkReader = new BinaryReader(stream);

            switch (chunkHeader.type)
            {
                //COLOR_256
                case FLI_COLOR_256_CHUNK:
                    frame.ColorMap = ReadColorChunk(chunkReader);
                    break;
                case FLI_BRUN_CHUNK:
                    ReadBrunChunk(chunkReader, frame, videoHeader);
                    break;
                case FLI_DELTA_CHUNK:
                    ReadDeltaChunk(chunkReader, frame, videoHeader);
                    break;
                //PSTAMP
/*                case 18:
                {
                    var chunkInfo = IOHelper.ReadTypeFromReader<FlcPStampChunk>(binReader);
                    break;
                }*/
            }
        }

        private static ColorRGB[] ReadColorChunk(BinaryReader binReader)
        {
            var colorMap = new ColorRGB[256];
            var numPackets = binReader.ReadUInt16();
            var i = 0;
            while (numPackets-- > 0)
            {
                i += IOHelper.ReadTypeFromReader<byte>(binReader); //colors to skip
                int numColors = binReader.ReadByte();
                if (numColors == 0)
                {
                    numColors = 256;
                }

                for (var j = 0; j < numColors && i + j < 256; ++j)
                {
                    var col = new ColorRGB()
                    {
                        r = binReader.ReadByte(),
                        g = binReader.ReadByte(),
                        b = binReader.ReadByte(),
                    };
                    colorMap[i + j] = col;
                }
            }

            return colorMap;
        }

        private static void ReadBrunChunk(BinaryReader binReader, FlcFrame frame, FlcHeader videoHeader)
        {
            var pixels = new byte[videoHeader.width * videoHeader.height];
            for (var y = 0; y < videoHeader.height; ++y)
            {
                var it = videoHeader.width * y;
                var numPackets = binReader.ReadByte();
                var x = 0;
                while (numPackets-- != 0 && x < videoHeader.width)
                {
                    /*
                     * The RLE scheme used in the BYTE_RUN packet is fairly simple.
                     * The first byte in each packet is a type byte that indicates how the packet data is to be interpreted.
                     * If the value of this byte is a positive number then the next byte is to be read and repeated "type" times.
                     * If the value is negative then it is converted to its absolute value and the next "type" pixels are read literally from the encoded data.
                     */
                    var count = binReader.ReadSByte();
                    if (count >= 0)
                    {
                        var col = binReader.ReadByte();
                        while (count-- != 0 && x < videoHeader.width)
                        {
                            pixels[it] = col;
                            ++it;
                            ++x;
                        }
                    }
                    else
                    {
                        while (count++ != 0 && x < videoHeader.width)
                        {
                            pixels[it] = binReader.ReadByte();
                            ++it;
                            ++x;
                        }
                    }
                }
            }

            frame.Pixels = pixels;
        }

        private static void ReadDeltaChunk(BinaryReader binReader, FlcFrame frame, FlcHeader videoHeader)
        {
            var numLines = binReader.ReadUInt16();
            var y = 0;
            while (numLines-- != 0)
            {
                var numPackets = 0;
                while (true)
                {
                    var word = binReader.ReadInt16();
                    if ((word & 0x8000) != 0) //bit 15 is set
                    {
                        if ((word & 0x4000) != 0) //bit 14 is set
                        {
                            y += -word; //skip lines
                        }
                        else // only last pixel has changed
                        {
                            if (y >= 0 && y < videoHeader.height)
                            {
                                frame.Pixels[y * videoHeader.width + videoHeader.width - 1] = (byte) (word & 0xff);
                            }

                            ++y;
                            if (numLines-- == 0)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        numPackets = word;
                        break;
                    }
                }

                if (y >= videoHeader.height)
                {
                    break;
                }

                var x = 0;
                while (numPackets-- != 0)
                {
                    x += binReader.ReadByte();
                    var count = binReader.ReadSByte();
                    var it = y * videoHeader.width + x;
                    if (count >= 0)
                    {
                        while (count-- != 0 && x < videoHeader.width)
                        {
                            var color1 = binReader.ReadByte();
                            var color2 = binReader.ReadByte();
                            frame.Pixels[it] = color1;
                            ++it;
                            ++x;
                            if (x >= videoHeader.width)
                            {
                                continue;
                            }

                            frame.Pixels[it] = color2;
                            ++it;
                            ++x;
                        }
                    }
                    else
                    {
                        var color1 = binReader.ReadByte();
                        var color2 = binReader.ReadByte();
                        while (count++ != 0 && x < videoHeader.width)
                        {
                            frame.Pixels[it] = color1;
                            ++it;
                            ++x;
                            if (x >= videoHeader.width)
                            {
                                continue;
                            }

                            frame.Pixels[it] = color2;
                            ++it;
                            ++x;
                        }
                    }
                }

                ++y;
            }
        }
    }
}
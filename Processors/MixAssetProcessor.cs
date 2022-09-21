using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WDViewer.Assets;
using static WDViewer.Reader.ReaderCommon;

namespace WDViewer.Reader
{
    internal class MixAssetProcessor : IAssetProcessor
    {
        private struct MixHeader
        {
            public uint unused;
            public uint streamsCount;
            public uint streamsOffset;
            public uint palettesCount;
            public uint palettesFirstIndex;
            public uint palettesOffset;
        };

        private struct SegmentedImageHeader
        {
            public uint width;
            public uint height;
            public uint dataBlockSize;
            public uint scanLinesCount;
            public uint segmentBlockSize;
            public uint unknown6;
            public uint unknown7;
            public uint unknown8;
            public uint unknown9;
        };

        private struct SegmentedImageSegment
        {
            public byte padding;
            public byte width;
        };


        public Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries)
        {
            using var stream = new MemoryStream(content);
            using var binReader = new BinaryReader(stream);

            var isMixFile = Match(binReader, "MIX FILE  ");
            if (!isMixFile)
            {
                return Tuple.Create<bool, Asset>(false, null);
            }

            Asset result = new AssetMix()
            {
                Path = path,
                Content = new List<Asset>(),
            };

            var header = IOHelper.ReadTypeFromReader<MixHeader>(binReader);
            var matchConstant = Match(binReader, "ENTRY");
            if (!matchConstant)
            {
                Debug.WriteLine("error reading MIX constant");
                return Tuple.Create(true, result);
            }

            var streamPositions = Enumerable.Range(0, (int) header.streamsCount)
                .Select(_ =>
                {
                    var entryPosition = IOHelper.ReadTypeFromReader<int>(binReader);
                    return entryPosition;
                }).ToList();

            var isPalette = Match(binReader, " PAL ");
            if (!isPalette)
            {
                Debug.WriteLine("no palette entry found");
                return Tuple.Create(true, result);
            }

            if (stream.Position != header.palettesOffset)
            {
                Debug.WriteLine($"palette offset mismatch {stream.Position}, expected {header.palettesOffset}");
            }

//            var paletteSize = ASSET_PALETTE_COUNT * Marshal.SizeOf<ColorRGB>();

            var palettes = Enumerable.Range(0, (int) header.palettesCount)
                .Select(i =>
                {
                    var palette = Enumerable.Range(0, (int) ASSET_PALETTE_COUNT)
                        .Select(_ => IOHelper.ReadTypeFromReader<ColorRGB>(binReader)).ToArray();
                    return new AssetPalette()
                    {
                        Path = $"{path}/{i}.PAL",
                        Palette = palette,
                    };
                }).ToList();

            if (header.streamsCount <= 0)
            {
                return Tuple.Create(true, result);
            }

            if (!Match(binReader, "DATA "))
            {
                Debug.WriteLine("no data entry found");
                return Tuple.Create(true, result);
            }

            var streamInitialPosition = stream.Position;

            ((AssetMix) result).Content.AddRange(streamPositions.Select((pos, i) =>
            {
                var streamStart = pos + streamInitialPosition;
                var streamEnd = i + 1 < header.streamsCount ? streamPositions[(int) i + 1] + streamInitialPosition : content.Length;
//                var streamSize = streamEnd - streamStart;
                stream.Seek(streamStart, SeekOrigin.Begin);

                var assetStart = streamStart;

                var imageSize = IOHelper.ReadTypeFromReader<ImageSize>(binReader);
                assetStart += Marshal.SizeOf<ImageSize>();

                var streamType = binReader.ReadByte();
                assetStart += 1;

                switch (streamType)
                {
                    case 1:
                    {
                        var paletteIndex = binReader.ReadByte();
                        assetStart += 1;

                        var imagePalette = palettes[paletteIndex - (int) header.palettesFirstIndex];
                        var assetSize = streamEnd - assetStart;
                        var content = binReader.ReadBytes((int) assetSize);

                        return new AssetImage
                        {
                            Path = $"8Bit Image {i}",
                            Width = imageSize.width,
                            Height = imageSize.height,
                            Image = CreatePaletteImage(imageSize.width, imageSize.height, content, imagePalette, true),
                            Palette = imagePalette,
                        };
                    }
                    case 2:
                    {
                        //skip unknown byte
                        stream.Seek(1, SeekOrigin.Current);

                        assetStart += 1;
                        var assetSize = streamEnd - assetStart;
                        var imageBytes = binReader.ReadBytes((int) assetSize);

                        return new AssetImage
                        {
                            Path = $"16Bit Image {i}",
                            Width = imageSize.width,
                            Height = imageSize.height,
                            Image = CreateImageRGB565(imageSize.width, imageSize.height, imageBytes),
                        };
                    }
                    case 9:
                    {
                        var paletteIndex = binReader.ReadByte();
                        assetStart += 1;

                        var imagePalette = palettes[paletteIndex - (int) header.palettesFirstIndex];


                        var segmentedImageHeader = IOHelper.ReadTypeFromReader<SegmentedImageHeader>(binReader);

                        if (imageSize.width != segmentedImageHeader.width || imageSize.height != segmentedImageHeader.height)
                        {
                            Debug.WriteLine(
                                $"error reading segment {segmentedImageHeader.width}:{segmentedImageHeader.height}, expected {imageSize.width}:{imageSize.height}");
                        }

                        //get scanline indices
                        var scanLines = Enumerable.Range(0, (int) segmentedImageHeader.scanLinesCount)
                            .Select(j => binReader.ReadUInt16()).ToList();

                        //get data offsets
                        var dataOffsets = Enumerable.Range(0, (int) segmentedImageHeader.scanLinesCount)
                            .Select(j => binReader.ReadUInt16()).ToList();

                        //get segments
                        var segmentSize = Marshal.SizeOf<SegmentedImageSegment>();
                        var segmentsAmount = segmentedImageHeader.segmentBlockSize / segmentSize;
                        var segments = Enumerable.Range(0, (int) segmentsAmount)
                            .Select(j => IOHelper.ReadTypeFromReader<SegmentedImageSegment>(binReader)).ToList();

                        //skip unknown byte
                        var unknownByte = binReader.ReadByte();
                        //                                   stream.Seek(1, SeekOrigin.Current);

                        var dataBlockOffset = stream.Position;
                        var dataBlockEnd = dataBlockOffset + segmentedImageHeader.dataBlockSize;

                        if (dataBlockEnd != streamEnd)
                        {
                            Debug.WriteLine($"error in data block {dataBlockEnd}, expected {streamEnd}");
                        }

                        //Create memory file to store image 8 bit palette indexes and set it as asset file
                        var imagePixelCount = segmentedImageHeader.width * segmentedImageHeader.height;
                        var assetSize = imagePixelCount;
                        assetStart = 0;

                        var imageBytes = new byte[assetSize];
                        using (var imageStream = new MemoryStream(imageBytes))
                        {
                            var lineIndex = 0;
                            for (var scanLineIndex = 0; scanLineIndex < segmentedImageHeader.scanLinesCount - 1; scanLineIndex++)
                            {
                                stream.Seek(dataBlockOffset + dataOffsets[scanLineIndex], SeekOrigin.Begin);

                                var lineSize = 0;
                                var lineEnd = scanLines[scanLineIndex + 1] / segmentSize;

                                for (var segmentIndex = scanLines[scanLineIndex] / segmentSize; segmentIndex < lineEnd; segmentIndex++)
                                {
                                    var segment = segments[segmentIndex];
                                    //add line size
                                    lineSize += segment.padding + segment.width;
                                    if (lineSize > segmentedImageHeader.width)
                                    {
                                        Debug.WriteLine(
                                            $"line size is bigger than image {lineSize}, expected {segmentedImageHeader.width}");
                                    }

                                    // fill left padding
                                    for (var j = 0; j < segment.padding; j++)
                                    {
                                        imageStream.WriteByte(0);
                                    }

                                    //write segment data
                                    var segmentBuffer = binReader.ReadBytes(segment.width);
                                    imageStream.Write(segmentBuffer);
                                }

                                //fill right buffer
                                for (var j = 0; j < segmentedImageHeader.width - lineSize; j++)
                                {
                                    if (imageStream.Position < assetSize)
                                    {
                                        imageStream.WriteByte(0);
                                    }
                                }

                                lineIndex++;
                            }
                        }

                        return new AssetImage
                        {
                            Path = $"Segmented Image {i}",
                            Width = imageSize.width,
                            Height = imageSize.height,
                            Image = CreatePaletteImage(imageSize.width, imageSize.height, imageBytes, imagePalette, false),
                            Palette = imagePalette,
                        };
                    }
                    default:
                    {
                        Debug.WriteLine($"unknown streamType {streamType}");
                        return new Asset
                        {
                            Path = $"unknown {streamType}",
                        };
                    }
                }
            }));
            ((AssetMix) result).Content.AddRange(palettes);

            return Tuple.Create(true, result);
        }

        private static bool Match(BinaryReader stream, string match)
        {
            var buffer = stream.ReadBytes(match.Length);
            var readName = Encoding.ASCII.GetString(buffer);
            var matching = readName.Equals(match);
            if (!matching)
            {
                Debug.WriteLine($"mix file header mismatch \"{readName}\", expected \"{match}\"");
            }

            return matching;
        }
    }
}
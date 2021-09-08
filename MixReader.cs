using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer
{
    class MixReader
    {

        private static uint ASSET_PALETTE_COUNT = 256;

        struct MixHeader
        {
            public uint unused;
            public uint streamsCount;
            public uint streamsOffset;
            public uint palettesCount;
            public uint palettesFirstIndex;
            public uint palettesOffset;
        };

        struct ImageSize
        {
            public ushort width;
            public ushort height;
        }

        struct SegmentedImageHader
        {
            public int width;
            public int height;
            public uint dataBlockSize;
            public uint scanLinesCount;
            public uint segmentBlockSize;
            public uint unknown6;
            public uint unknown7;
            public uint unknown8;
            public uint unknown9;
        };

        struct SegmentedImageSegment
        {
            public byte padding;
            public byte width;
        };


        public List<Entry> Read(byte[] content)
        {
            using var stream = new MemoryStream(content);
            using var binReader = new BinaryReader(stream);
            var isMixFile = Match(binReader, "MIX FILE  ");
            if (!isMixFile)
            {
                Debug.WriteLine("no mix file selected");
                return new List<Entry>();
            }
            var header = Helper.ReadTypeFromReader<MixHeader>(binReader);
            var matchConstant = Match(binReader, "ENTRY");
            if (!matchConstant)
            {
                Debug.WriteLine("error reading MIX constant");
            }

            var streamPositions = Enumerable.Range(0, (int)header.streamsCount)
                .Select(i =>
                {
                    var entryPosition = Helper.ReadTypeFromReader<int>(binReader);
                    return entryPosition;
                }).ToList();
            Debug.WriteLine($"mix file entries: {streamPositions.Count}");

            var isPalette = Match(binReader, " PAL ");
            if (!isPalette)
            {
                Debug.WriteLine("no palette entry found");
                return new List<Entry>();
            }
            if (stream.Position != header.palettesOffset)
            {
                Debug.WriteLine($"palette offset mismatch {stream.Position}, expected {header.palettesOffset}");
            }

            var paletteSize = ASSET_PALETTE_COUNT * Marshal.SizeOf<ColorRGB>();

            var palettes = Enumerable.Range(0, (int)header.palettesCount)
                .Select(i =>
                {
                    var palette = Enumerable.Range(0, (int)ASSET_PALETTE_COUNT).Select(c => Helper.ReadTypeFromReader<ColorRGB>(binReader)).ToArray();
                    return new AssetPalette()
                    {
                        Path = $"{i}.PAL",
                        Palette = palette,
                    };
                }).ToList();
            var res = new List<Entry>();
            if (header.streamsCount > 0)
            {
                if (!Match(binReader, "DATA "))
                {
                    Debug.WriteLine("no data entry found");
                    return new List<Entry>();
                }

                var streamInitialPosition = stream.Position;

                res = streamPositions.Select((pos, i) =>
                  {
                      var streamStart = pos + streamInitialPosition;
                      var streamEnd = i + 1 < header.streamsCount ? streamPositions[(int)i + 1] + streamInitialPosition : content.Length;
                      var streamSize = streamEnd - streamStart;
                      stream.Seek(streamStart, SeekOrigin.Begin);

                      var assetStart = streamStart;

                      var imageSize = Helper.ReadTypeFromReader<ImageSize>(binReader);
                      assetStart += Marshal.SizeOf<ImageSize>();

                      var streamType = Helper.ReadTypeFromReader<byte>(binReader);
                      assetStart += 1;

                      switch (streamType)
                      {
                          case 1:
                              {
                                  var paletteIndex = Helper.ReadTypeFromReader<byte>(binReader);
                                  assetStart += 1;

                                  Debug.WriteLine($"palette index {paletteIndex}");

                                  var imagePalette = palettes[paletteIndex - (int)header.palettesFirstIndex];
                                  var assetSize = streamEnd - assetStart;
                                  var content = binReader.ReadBytes((int)assetSize);

                                  return new EntryImage
                                  {
                                      Name = $"8Bit Image {i}",
                                      Data = content,
                                      Width = imageSize.width,
                                      Height = imageSize.height,
                                      Image = CreatePaletteImage(imageSize.width, imageSize.height, content, imagePalette),
                                  };
                              }
                          case 2:
                              {
                                  //skip unknown byte
                                  stream.Seek(1, SeekOrigin.Current);

                                  assetStart += 1;
                                  var assetSize = streamEnd - assetStart;
                                  var imageBytes = binReader.ReadBytes((int)assetSize);

                                  return new EntryImage
                                  {
                                      Name = $"16Bit Image {i}",
                                      Width = imageSize.width,
                                      Height = imageSize.height,
                                      Image = CreateImageRGB565(imageSize.width, imageSize.height, imageBytes),
                                  };
                              }
                          case 9:
                              {
                                  var paletteIndex = Helper.ReadTypeFromReader<byte>(binReader);
                                  assetStart += 1;

                                  var imagePalette = palettes[paletteIndex - (int)header.palettesFirstIndex];


                                  var segmentedImageHeader = Helper.ReadTypeFromReader<SegmentedImageHader>(binReader);

                                  if (imageSize.width != segmentedImageHeader.width || imageSize.height != segmentedImageHeader.height)
                                  {
                                      Debug.WriteLine($"error reading segment {segmentedImageHeader.width}:{segmentedImageHeader.height}, expected {imageSize.width}:{imageSize.height}");
                                  }

                                  //get scanline indices
                                  var scanLines = Enumerable.Range(0, (int)segmentedImageHeader.scanLinesCount).Select(j => Helper.ReadTypeFromReader<ushort>(binReader)).ToList();

                                  //get data offsets
                                  var dataOffsets = Enumerable.Range(0, (int)segmentedImageHeader.scanLinesCount).Select(j => Helper.ReadTypeFromReader<ushort>(binReader)).ToList();

                                  //get segments
                                  var segmentSize = Marshal.SizeOf<SegmentedImageSegment>();
                                  var segmentsAmount = segmentedImageHeader.segmentBlockSize / segmentSize;
                                  var segments = Enumerable.Range(0, (int)segmentsAmount).Select(j => Helper.ReadTypeFromReader<SegmentedImageSegment>(binReader)).ToList();

                                  //skip unknown byte
                                  stream.Seek(1, SeekOrigin.Current);

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
                                                  Debug.WriteLine($"line size is bigger than image {lineSize}, expected {segmentedImageHeader.width}");
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

                                  return new EntryImage
                                  {
                                      Name = $"Segmented Image {i}",
                                      Width = imageSize.width,
                                      Height = imageSize.height,
                                      Image = CreatePaletteImage(imageSize.width, imageSize.height, imageBytes, imagePalette),
                                  };
                              }
                          default:
                              {
                                  Debug.WriteLine($"unknown streamType {streamType}");
                                  return new Entry
                                  {
                                      Name = $"unknown {streamType}",
                                  };
                              }
                      }
                  }).ToList();
            }

            Debug.WriteLine($"mix file read successfully");
            return res;
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

        private static Image CreateImageRGB565(int width, int height, byte[] imageBytes)
        {
            var img = new Bitmap(width, height);
            var data = img.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            var ptr = data.Scan0;
            var size = Math.Abs(data.Stride) * height;
            Marshal.Copy(imageBytes, 0, ptr, size);
            img.UnlockBits(data);
            return img;
        }

        private static Image CreatePaletteImage(int width, int height, byte[] imageBytes, AssetPalette palette)
        {
            var img = new Bitmap(width, height);
            var data = img.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var ptr = data.Scan0;
            var size = Math.Abs(data.Stride) * height;

            var colorBytes = new byte[size];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = palette.Palette[imageBytes[x + y * width]];
                    colorBytes[(x + y * width) * 4 + 3] = 255;
                    colorBytes[(x + y * width) * 4 + 2] = color.r;
                    colorBytes[(x + y * width) * 4 + 1] = color.g;
                    colorBytes[(x + y * width) * 4 + 0] = color.b;
                }
            }
            Marshal.Copy(colorBytes, 0, ptr, size);
            img.UnlockBits(data);
            return img;
        }
    }
}

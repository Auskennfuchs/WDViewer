using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WDViewer.Assets;

namespace WDViewer.Reader
{
    internal class WdFileReader
    {
        private struct WdFileRecord
        {
            public UInt32 FileOffset;
            public Int32 FileSize;
            public UInt32 Unused1;
            public UInt32 Unused2;
            public UInt32 CheckSum;
            public UInt32 NameOffset;
        };

        private struct FileAsset
        {
            public string Path { get; set; }
            public int FileSize { get; set; }
            public uint FileOffset { get; set; }

            public byte[] Content { get; set; }
        };

        private readonly int recordSize = Marshal.SizeOf<WdFileRecord>();

        private uint nameBlockSize;
        private byte[] namesBlock;

        private readonly List<IAssetProcessor> processors;

        public WdFileReader()
        {
            processors = new List<IAssetProcessor>
            {
                new PcxProcessor(),
                new PalProcessor(),
                new LevelDatProcessor(),
                new ImageDatProcessor(),
                new MixAssetProcessor(),
                new FlcProcessor()
            };
        }

        public Dictionary<string, Asset> Read(string fileName)
        {
            var fileExtName = Path.GetFileName(fileName).ToLower();
            if (fileExtName == "smp0.wd" || fileExtName == "smp1.wd")
            {
                return ReadPcmFile(fileName);
            }

            return ReadStandardWdFile(fileName);
        }

        private Dictionary<string, Asset> ReadStandardWdFile(string fileName)
        {
            var rawFileName = Path.GetFileNameWithoutExtension(fileName);
            using var file = File.OpenRead(fileName);
            using var fileStream = new BinaryReader(file);
            var recordCount = fileStream.ReadUInt32();
            var recordBlockOffset = file.Position;

            if (recordCount * recordSize > file.Length)
            {
                throw new IOException("No Earth2140 WD-file");
            }

            file.Seek(recordCount * recordSize, SeekOrigin.Current);

            nameBlockSize = fileStream.ReadUInt32();

            if (nameBlockSize == 0)
            {
                throw new IOException("name block is null");
            }

            namesBlock = fileStream.ReadBytes((int) nameBlockSize);
            if (namesBlock[nameBlockSize - 1] != '\0')
            {
                throw new IOException("Name Block doesn't end with null terminator");
            }

            file.Seek(recordBlockOffset, SeekOrigin.Begin);
            var fileEntry = Enumerable.Range(0, (int) recordCount)
                .Select((recordIndex) =>
                {
                    var recordBytes = fileStream.ReadBytes(recordSize);
                    var record = IOHelper.ByteToType<WdFileRecord>(recordBytes);

                    var recordName = ExtractName(record.NameOffset);

                    return new FileAsset()
                    {
                        Path = recordName,
                        FileSize = record.FileSize,
                        FileOffset = record.FileOffset,
                    };
                })
                .ToList();

            var result = new Dictionary<string, Asset>();
            foreach (var entry in fileEntry)
            {
                file.Seek(entry.FileOffset, SeekOrigin.Begin);
                var content = fileStream.ReadBytes(entry.FileSize);
                var entryName = rawFileName + "/" + entry.Path;
                result.Add(entryName, new RawAsset()
                {
                    Path = entryName,
                    Content = content,
                });
            }

            return processors.Aggregate(result, (current, processor) => current
                .Select(entry =>
                {
                    if (entry.Value is not RawAsset rawEntry)
                    {
                        return entry;
                    }

                    var (success, asset) = processor.Read(rawEntry.Content, entry.Key, current);
                    return success ? new KeyValuePair<string, Asset>(entry.Key, asset) : entry;
                })
                .Where(e => e.Value != null) //remove empty values if removed inside processor
                .ToDictionary(e => e.Key, e => e.Value));
        }

        private static Dictionary<string, Asset> ReadPcmFile(string fileName)
        {
            var rawFileName = Path.GetFileNameWithoutExtension(fileName);
            using var file = File.OpenRead(fileName);
            using var fileStream = new BinaryReader(file);
            var bytes = fileStream.ReadBytes((int) file.Length);
            return new Dictionary<string, Asset>()
            {
                {
                    fileName, new AssetAudio()
                    {
                        Path = rawFileName,
                        PcmData = bytes,
                    }
                }
            };
        }

        private string ExtractName(uint offset)
        {
            var recordName = "";
            for (var nameIndex = offset; nameIndex < nameBlockSize; ++nameIndex)
            {
                if (namesBlock[nameIndex] == '\0')
                {
                    break;
                }

                recordName += Convert.ToChar(namesBlock[nameIndex]);
            }

            return recordName;
        }
    }
}
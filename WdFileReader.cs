using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer
{
    class WdFileReader
    {
        struct WDFileRecord
        {
            public UInt32 FileOffset;
            public Int32 FileSize;
            public UInt32 Unused1;
            public UInt32 Unused2;
            public UInt32 CheckSum;
            public UInt32 NameOffset;
        };
        int recordSize = Marshal.SizeOf<WDFileRecord>();

        private uint nameBlockSize;
        private byte[] namesBlock;

        public List<Asset> Read(string fileName)
        {
            using var file = File.OpenRead(fileName);
            using var fileStream = new BinaryReader(file);
            var recordCount = fileStream.ReadUInt32();
            var recordBlockOffset = file.Position;

            file.Seek(recordCount * recordSize, SeekOrigin.Current);

            nameBlockSize = fileStream.ReadUInt32();

            namesBlock = fileStream.ReadBytes((int)nameBlockSize);
            if (namesBlock[nameBlockSize - 1] != '\0')
            {
                throw new IOException("Name Block doesn't end with null terminator");
            }

            file.Seek(recordBlockOffset, SeekOrigin.Begin);
            var assets = Enumerable.Range(0, (int)recordCount)
                         .Select((recordIndex) =>
                         {
                             var recordBytes = fileStream.ReadBytes(recordSize);
                             var record = Helper.ByteToType<WDFileRecord>(recordBytes);

                             var recordName = ExtractName(record.NameOffset);
                             Console.WriteLine(recordName);

                             return new Asset()
                             {
                                 Path = recordName,
                                 FileSize = record.FileSize,
                                 FileOffset = record.FileOffset,
                             };
                         })
                         .ToList();
            foreach (var asset in assets)
            {
                file.Seek(asset.FileOffset, SeekOrigin.Begin);
                asset.Content = fileStream.ReadBytes(asset.FileSize);
            }
            return assets;
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

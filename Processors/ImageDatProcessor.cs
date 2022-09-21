using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WDViewer.Assets;
using static WDViewer.Reader.ReaderCommon;

namespace WDViewer.Reader
{
    public class ImageDatProcessor : IAssetProcessor
    {
        public Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries)
        {
            var ext = path.Substring(path.Length - 4, 4);
            var fileWithoutExt = path[0..^4];

            if (ext != ".DAT")
            {
                return Tuple.Create<bool, Asset>(false, null);
            }
            if (!entries.ContainsKey(fileWithoutExt + ".PAL"))
            {
                Debug.WriteLine($"no palette for DAT-file {path}");
                return Tuple.Create<bool, Asset>(false, null);
            }
            var pal = entries[fileWithoutExt + ".PAL"];
            if (!(pal is AssetPalette))
            {
                Debug.WriteLine($"no palette for DAT-file {path}");
                return Tuple.Create<bool, Asset>(false, null);
            }

            using var stream = new MemoryStream(content);
            using var binReader = new BinaryReader(stream);

            var imageSize = IOHelper.ReadTypeFromReader<ImageSize>(binReader);

            // skip unknown
            stream.Seek(2, SeekOrigin.Current);

            var imageByteSize = imageSize.width * imageSize.height;
            var imageBytes = binReader.ReadBytes(imageByteSize);

            return Tuple.Create(true, (Asset)new AssetImage()
            {
                Width = imageSize.width,
                Height = imageSize.height,
                Palette = pal as AssetPalette,
                Image = CreatePaletteImage(imageSize.width, imageSize.height, imageBytes, pal as AssetPalette, false),
                Path = path,
            });
        }
    }
}

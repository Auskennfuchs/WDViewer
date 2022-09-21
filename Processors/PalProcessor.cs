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
    class PalProcessor : IAssetProcessor
    {
        public Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries)
        {
            var ext = path.Substring(path.Length - 4, 4);

            if (ext != ".PAL")
            {
                return Tuple.Create<bool, Asset>(false, null);
            }

            using var stream = new MemoryStream(content);
            using var binReader = new BinaryReader(stream);

            var paletteSize = ReaderCommon.ASSET_PALETTE_COUNT * Marshal.SizeOf<ColorRGB>();

            var palette = Enumerable.Range(0, (int)ReaderCommon.ASSET_PALETTE_COUNT).Select(c => IOHelper.ReadTypeFromReader<ColorRGB>(binReader)).ToArray();
            return Tuple.Create(true,
                (Asset)new AssetPalette()
                {
                    Path = path,
                    Palette = palette,
                });
        }
    }
}

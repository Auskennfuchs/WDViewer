using Kaitai;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDViewer.Assets;


namespace WDViewer.Reader
{
    class PcxProcessor : IAssetProcessor
    {
        public Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries)
        {
            var ext = path.Substring(path.Length - 4, 4);

            if (ext != ".PCX")
            {
                return Tuple.Create<bool, Asset>(false, null);
            }

            var pcx = new Pcx();
            try
            {
                using var stream = new MemoryStream(content);
                pcx.ReadFromStream(stream, -1, -1);
                var image = ReaderCommon.CreateImageRGBA(pcx.Width, pcx.Height, pcx.RgbaData);

                return Tuple.Create<bool, Asset>(true, new AssetImage()
                {
                    Height = pcx.Height,
                    Width = pcx.Width,
                    Image = image,
                    Path = path,
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"error reading pcx {e.Message}");
                return Tuple.Create<bool, Asset>(false, null);
            }
        }
    }
}

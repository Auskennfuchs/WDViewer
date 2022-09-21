using System.Drawing;
using WDViewer.Reader.Flc;

namespace WDViewer.Assets
{
    public class AssetVideo : Asset
    {
        public uint Width { get; set; }
        public uint Height { get; set; }

        public uint NumFrames { get; set; }
        public uint Delay { get; set; }

        public Image[] Frames { get; set; }
    }
}
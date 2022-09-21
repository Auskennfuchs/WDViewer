using System.Drawing;

namespace WDViewer.Reader.Flc
{
    public class FlcFrame
    {
        public ColorRGB[] ColorMap { get; set; }

        public byte[] Pixels { get; set; }
    }
}
namespace WDViewer.Reader.Flc
{
    public struct FlcColor
    {
        public byte r, g, b;

        public FlcColor(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    public class FlcColorMap
    {
        private const int Size = 256;
        private readonly FlcColor[] colors;

        public FlcColorMap()
        {
            colors = new FlcColor[Size];
        }

        public FlcColor this[int idx]
        {
            get => colors[idx];
            set => colors[idx] = value;
        }
    }
}
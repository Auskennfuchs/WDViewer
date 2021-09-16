using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.Assets;

namespace WDViewer.Controls
{
    public partial class PaletteControl : UserControl
    {

        public ColorRGB[] Palette { get; set; }
        public PaletteControl()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
        }

        private void PaletteControl_Paint(object sender, PaintEventArgs e)
        {
            if (Palette == null)
            {
                return;
            }

            using var graphics = this.CreateGraphics();

            var aspect = Width / (float)Height;

            var squareNum = Math.Sqrt(Palette.Length);

            var colsPerColumn = (int)Math.Max(1, Math.Min(Palette.Length, Math.Round(squareNum * aspect)));
            var colsPerRow = (int)Math.Ceiling(Palette.Length / (float)colsPerColumn);

            var colWidth = this.Width / colsPerColumn;
            var colHeight = this.Height / colsPerRow;
            foreach (var (pal, idx) in Palette.Select((item, index) => (item, index)))
            {
                var brush = new SolidBrush(Color.FromArgb(pal.r, pal.g, pal.b));
                graphics.FillRectangle(brush, (idx % colsPerColumn) * colWidth, idx / colsPerColumn * colHeight, colWidth, colHeight);
            }
        }
    }
}

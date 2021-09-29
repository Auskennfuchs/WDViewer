using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.Assets;

namespace WDViewer.Controls
{
    public partial class ImageControl : UserControl
    {

        public AssetImage Image
        {
            set
            {
                assetImage = value;
                assetImageControl.Image = value.Image;
                paletteControl.Palette = value.Palette?.Palette;
            }
        }

        private AssetImage assetImage;

        public ImageControl()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            exportFileDialog.FileName = assetImage.Path + ".png";
            var result = exportFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var pngEncoderInfo = GetEncoderInfo("image/png");
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                var bitmap = new Bitmap(assetImage.Image);
                bitmap.Save(exportFileDialog.FileName, pngEncoderInfo, encoderParameters);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private Color GetPixel(int mX, int mY)
        {
            var imageWidth = assetImage.Image.Width;
            var imageHeight = assetImage.Image.Height;
            var boxWidth = assetImageControl.ClientSize.Width;
            var boxHeight = assetImageControl.ClientSize.Height;
            int x;
            int y;

            float pic_aspect = boxWidth / (float)boxHeight;
            float img_aspect = imageWidth / (float)imageHeight;
            if (pic_aspect > img_aspect)
            {
                // The PictureBox is wider/shorter than the image.
                y = (int)(imageHeight * mY / (float)boxHeight);

                // The image fills the height of the PictureBox.
                // Get its width.
                float scaled_width = imageWidth * boxHeight / imageHeight;
                float dx = (boxWidth - scaled_width) / 2;
                x = (int)((mX - dx) * imageHeight / (float)boxHeight);
            }
            else
            {
                // The PictureBox is taller/thinner than the image.
                x = (int)(imageWidth * mX / (float)boxWidth);

                // The image fills the height of the PictureBox.
                // Get its height.
                float scaled_height = imageHeight * boxWidth / imageWidth;
                float dy = (boxHeight - scaled_height) / 2;
                y = (int)((mY - dy) * imageWidth / boxWidth);
            }
            if (x < 0 || x >= imageWidth ||
                y < 0 || y >= imageHeight)
            {
                return Color.Transparent;
            }

            using var bitmap = new Bitmap(assetImage.Image);
            return bitmap.GetPixel(x, y);
        }

        private void assetImageControl_MouseMove(object sender, MouseEventArgs e)
        {
            var pixel = GetPixel(e.X, e.Y);
            lblCurColor.Text = $"({pixel.R},{pixel.G},{pixel.B},{pixel.A})";
            curColPanel.BackColor = pixel;
        }
    }
}

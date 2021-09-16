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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.Assets;

namespace WDViewer.Controls
{
    public partial class MixControl : UserControl
    {

        private class AssetListItem
        {
            public string Name { get; set; }
            public Asset Asset { get; set; }
        }

        public List<Asset> Assets
        {
            get
            {
                return assets;
            }
            set
            {
                if (value != null)
                {
                    assets = value;
                }
                else
                {
                    assets = new List<Asset>();
                }
                contentListBox.Items.AddRange(assets.Select(asset => new AssetListItem()
                {
                    Name = asset.Path,
                    Asset = asset,
                }).ToArray());

            }
        }

        private List<Asset> assets;


        public MixControl()
        {
            InitializeComponent();
            contentListBox.DisplayMember = "Name";
            contentListBox.Focus();
        }

        private void contentListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedEntry = (AssetListItem)contentListBox.SelectedItem;
            if (selectedEntry.Asset is Asset entry)
            {
                previewPanel.Controls.Clear();
                if (entry is AssetImage entryImg)
                {
                    var imageBox = new ImageControl()
                    {
                        Dock = DockStyle.Fill,
                        Image = entryImg,
                    };
                    previewPanel.Controls.Add(imageBox);
                }
                else if (entry is AssetPalette pal && pal.Palette != null)
                {
                    var paletteControl = new PaletteControl()
                    {
                        Dock = DockStyle.Fill,
                        Palette = pal.Palette,
                    };
                    previewPanel.Controls.Add(paletteControl);
                }
            }

        }

        private void bntExportAll_Click(object sender, EventArgs e)
        {
            var result = exportAllDialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            using var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var a in Assets)
                {
                    if (a is AssetImage assetImage)
                    {
                        var imageEntry = zipArchive.CreateEntry(assetImage.Path + ".png");
                        using var entryStream = imageEntry.Open();
                        WriteBitmap(assetImage, entryStream);
                    }
                }
            }
            using var fileStream = new FileStream(exportAllDialog.FileName, FileMode.CreateNew);
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(fileStream);
        }

        private static void WriteBitmap(AssetImage assetImage, Stream outputStream)
        {
            var pngEncoderInfo = GetEncoderInfo("image/png");
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            var bitmap = new Bitmap(assetImage.Image);
            bitmap.Save(outputStream, pngEncoderInfo, encoderParameters);
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

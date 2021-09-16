using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
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
    }
}

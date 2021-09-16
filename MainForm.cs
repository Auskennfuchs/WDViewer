using Be.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.AssetControls;
using WDViewer.Assets;
using WDViewer.Controls;
using WDViewer.Reader;

namespace WDViewer
{
    public partial class MainForm : Form
    {
        private WdFileReader fileReader;

        private MixAssetProcessor mixReader;

        private List<Asset> assets;

        private class AssetListItem
        {
            public string Name { get; set; }
            public Asset Asset { get; set; }
        }

        private class EntryListItem
        {
            public string Name { get; set; }
            public Asset Entry { get; set; }
        }

        public MainForm()
        {
            InitializeComponent();
            fileReader = new WdFileReader();
            mixReader = new MixAssetProcessor();
            contentFileListView.DisplayMember = "Name";
        }

        private void OnMenuOpenFile(object sender, EventArgs e)
        {
            var result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ResetView();
                string[] wdFiles = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.wd", new EnumerationOptions()
                {
                    MatchCasing = MatchCasing.CaseInsensitive,
                });
                var assets = wdFiles.Select(fileName =>
                {
                    return fileReader.Read(fileName);
                })
                    .Aggregate(new List<Asset>(), (res, a) =>
                    {
                        res.AddRange(a);
                        return res;
                    });
                contentFileListView.Items.Clear();
                contentFileListView.Items.AddRange(
                    assets.Select(asset => new AssetListItem()
                    {
                        Name = asset.Path,
                        Asset = asset,
                    })
                    .ToArray()
                );
            }
            /*            var result = openFileDialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            ResetView();
                            try
                            {
                                var fileName = openFileDialog.FileName;
                                assets = fileReader.Read(fileName);
                                contentFileListView.Items.Clear();
                                contentFileListView.Items.AddRange(
                                    assets.Select(asset => new AssetListItem()
                                    {
                                        Name = asset.Path,
                                        Asset = asset,
                                    })
                                    .ToArray()
                                );
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show(ex.Message, "Error reading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }*/
        }

        private void OnFileListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (AssetListItem)contentFileListView.SelectedItem;
            previewPanel.Controls.Clear();
            if (selectedItem == null)
            {
                return;
            }
            if (selectedItem.Asset is AssetMix mix)
            {
                previewPanel.BorderStyle = BorderStyle.None;
                var mixContent = new MixControl()
                {
                    Dock = DockStyle.Fill,
                    Assets = mix.Content,
                };
                previewPanel.Controls.Add(mixContent);
            }
            else if (selectedItem.Asset is AssetPalette pal)
            {
                var palContent = new PaletteControl()
                {
                    Palette = pal.Palette,
                    Dock = DockStyle.Fill,
                };
                previewPanel.Controls.Add(palContent);
            }
            else if (selectedItem.Asset is AssetImage img)
            {
                var palContent = new ImageControl()
                {
                    Image = img,
                    Dock = DockStyle.Fill,
                };
                previewPanel.Controls.Add(palContent);
            }
            else if (selectedItem.Asset is AssetAudio audio)
            {
                var palContent = new PcmAudioControl()
                {
                    Asset = audio,
                    Dock = DockStyle.Fill,
                };
                previewPanel.Controls.Add(palContent);
            }
            else if (selectedItem.Asset is RawAsset raw)
            {
                var hexContent = new HexViewControl()
                {
                    Path = raw.Path,
                    Content = raw.Content,
                    Dock = DockStyle.Fill,
                };
                previewPanel.Controls.Add(hexContent);
            }
            else
            {
                previewPanel.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void ResetView()
        {
            contentFileListView.Items.Clear();
            previewPanel.Controls.Clear();
        }
    }
}

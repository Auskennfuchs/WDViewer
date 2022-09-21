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

        private Dictionary<string, Asset> assets;

        public MainForm()
        {
            InitializeComponent();
            fileReader = new WdFileReader();
            contentFileListView.DisplayMember = "Name";
        }

        private void OnMenuOpenFile(object sender, EventArgs e)
        {
            var result = folderBrowserDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            ResetView();
            var wdFiles = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.wd", new EnumerationOptions()
            {
                MatchCasing = MatchCasing.CaseInsensitive,
            });
            assets = wdFiles.Select(fileName => fileReader.Read(fileName))
                .Aggregate(new Dictionary<string, Asset>(), (res, a) =>
                {
                    AddRange(res, a);
                    return res;
                });
            contentFileListView.Items.Clear();
            contentFileListView.Items.AddRange(
                assets.Values.Select(asset => new AssetListItem()
                {
                    Name = asset.Path,
                    Asset = asset,
                })
                    .ToArray()
            );
        }

        private static void AddRange<T>(ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
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
            else if (selectedItem.Asset is AssetLevel level)
            {
                var levelControl = new LevelControl(level, assets)
                {
                    Dock = DockStyle.Fill,
                };
                previewPanel.Controls.Add(levelControl);
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
            else if (selectedItem.Asset is AssetVideo video)
            {
                var videoContent = new AssetVideoControl()
                {
                    Video = video,
                    Dock = DockStyle.Fill,
                };
                previewPanel.Controls.Add(videoContent);
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDViewer
{
    public partial class MainForm : Form
    {
        private WdFileReader fileReader;

        private MixReader mixReader;

        private List<Asset> assets;

        private class AssetListItem
        {
            public string Name { get; set; }
            public Asset Asset { get; set; }
        }

        private class EntryListItem
        {
            public string Name { get; set; }
            public Entry Entry { get; set; }
        }

        public MainForm()
        {
            InitializeComponent();
            fileReader = new WdFileReader();
            mixReader = new MixReader();
            contentFileListView.DisplayMember = "Name";
            entriesListView.DisplayMember = "Name";
        }

        private void OnMenuOpenFile(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var fileName = openFileDialog.FileName;
                assets = fileReader.Read(fileName);
                Debug.WriteLine($"Assets: {assets.Count}");
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
        }

        private void OnFileListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            AssetListItem selectedItem = (AssetListItem)contentFileListView.SelectedItem;
            Debug.WriteLine($"load mix file {selectedItem.Name}");
            var entries = mixReader.Read(selectedItem.Asset.Content);
            entriesListView.Items.Clear();
            entriesListView.Items.AddRange(
                entries.Select(entry => new EntryListItem()
                {
                    Name = entry.Name,
                    Entry = entry,
                })
                .ToArray()
            );
            if(entriesListView.Items.Count>0)
            {
                entriesListView.SelectedIndex = 0;
                entriesListView.Focus();
            }
        }

        private void EntriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedEntry = (EntryListItem)entriesListView.SelectedItem;
            if (selectedEntry.Entry is Entry entry)
            {
                if (entry is EntryImage entryImg && entryImg.Image != null)
                {
                    previewPanel.Controls.Clear();
                    var imageBox = new PictureBox()
                    {
                        Parent=previewPanel,
                        Dock=DockStyle.Fill,
                        Image=entryImg.Image,
                        SizeMode=PictureBoxSizeMode.Zoom,
                    };
                    previewPanel.Controls.Add(imageBox);
                }
                else
                {
                }
            }
        }
    }
}

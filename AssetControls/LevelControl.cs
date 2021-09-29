using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.Assets;

namespace WDViewer.AssetControls
{
    public partial class LevelControl : UserControl
    {
        private LevelPictureBox levelPictureBox;
        public LevelControl(AssetLevel level, Dictionary<string, Asset> assets)
        {
            InitializeComponent();
            levelPictureBox = new LevelPictureBox(level, assets);

            mapPanel.Controls.Add(levelPictureBox);
        }

        private void chkShowFlags_CheckedChanged(object sender, EventArgs e)
        {
            levelPictureBox.ShowFlags = chkShowFlags.Checked;
        }
    }
}

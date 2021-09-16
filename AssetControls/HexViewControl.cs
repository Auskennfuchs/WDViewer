using Be.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDViewer.AssetControls
{
    public partial class HexViewControl : UserControl
    {

        public String Path { get; set; }

        public byte[] Content
        {
            set
            {
                content = value;
                hexBox.ByteProvider = new DynamicByteProvider(value);
            }
        }

        private byte[] content;

        public HexViewControl()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            exportFileDialog.FileName = Path;
            var result = exportFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllBytes(exportFileDialog.FileName, content);
            }

        }
    }
}

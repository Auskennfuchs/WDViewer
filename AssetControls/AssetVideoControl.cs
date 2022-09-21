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
    public partial class AssetVideoControl : UserControl
    {
        public AssetVideo Video
        {
            get => video;
            set
            {
                video = value;
                frameTimer.Stop();
                frameTimer.Interval = (int) value.Delay;
                StartVideo();
            }
        }

        private AssetVideo video;

        private uint currentFrame = 0;

        public AssetVideoControl()
        {
            InitializeComponent();
            videoImage.Dock = DockStyle.Fill;
            videoImage.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void OnFrameTick(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % video.NumFrames;
            videoImage.Image = video.Frames[currentFrame];
        }

        private void StartVideo()
        {
            videoImage.Image = video.Frames[currentFrame];
            frameTimer.Start();
        }
    }
}
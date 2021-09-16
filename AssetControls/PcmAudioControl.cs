using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.Assets;

namespace WDViewer.AssetControls
{
    public partial class PcmAudioControl : UserControl
    {
        public AssetAudio Asset { get; set; }

        private WaveOutEvent waveEvent;

        private Task task;
        public PcmAudioControl()
        {
            InitializeComponent();
            waveEvent = new WaveOutEvent();
            btnStop.Enabled = false;
        }

        private void onRemoveControl(object sender, ControlEventArgs e)
        {
            if (waveEvent.PlaybackState == PlaybackState.Playing)
            {
                waveEvent.Stop();
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
            Parent.ControlRemoved += onRemoveControl;
            var ms = new MemoryStream(Asset.PcmData);
            var rs = new RawSourceWaveStream(ms, new WaveFormat(16000, 8, 1));
            waveViewer.WaveStream = rs;
            waveEvent.Init(rs);
            task = Task.Run(() =>
              {
                  waveEvent.Play();
                  while (waveEvent.PlaybackState == PlaybackState.Playing)
                  {
                      Thread.Sleep(500);
                  }
                  waveEvent.Dispose();
              });
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            waveEvent.Stop();
            btnStop.Enabled = false;
            btnPlay.Enabled = true;
        }
    }
}

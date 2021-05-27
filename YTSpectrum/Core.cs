using Konamiman.Z80dotNet;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSpectrum
{
    public partial class Core
    {
        private static readonly int baseFreq = 96000;

        public Core(PictureBox screen) : this(new Core_Video(screen))
        {
            Task.Run(audioInSampler.StartRecording);
        }

    }
}

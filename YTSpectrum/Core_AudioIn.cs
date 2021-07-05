using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSpectrum
{
    class Core_AudioIn { }

    public partial class Core
    {
        private readonly WaveIn audioInSampler;
        private bool audioInState;

        private Core(Core_AudioIn _) : this(new Core_AudioOut())
        {
            audioInState = false;
            audioInSampler = new WaveIn()
            {
                WaveFormat = new WaveFormat(baseFreq, 8, 1)
            };
            audioInSampler.DataAvailable += AudioIn_DataAvailable;            
        }

        private void AudioIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int i = 0; i < e.BytesRecorded; i++)
            {
                audioInState = e.Buffer[i] > 150;
                CPU_Execute_Block();
            }
        }
    }

}

using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSpectrum
{
    class Core_AudioOut { }

    public partial class Core : WaveStream
    {
        private static readonly WaveFormat audioOutWaveFormat = new WaveFormat(baseFreq, 8, 1);
        private static readonly byte[] audioOutSpkLevels = { 128, 137, 209, 213 };

        public override WaveFormat WaveFormat => audioOutWaveFormat;
        public override long Length => long.MaxValue;
        public override long Position { get => audioOutPosition; set => throw new NotImplementedException(); }
        private long audioOutPosition = 0;

        private byte audioOutSpkValue;
        private ulong audioOutCount;
        private bool audioOutWaiting;
        private byte audioOutNextSpkValue;
        private ulong audioOutChange;
        private ulong audioOutLatency;        
        private readonly Queue<ulong> audioOutEvents = new Queue<ulong>();

        private Core(Core_AudioOut _) : this(new Core_IO())
        {
            audioOutSpkValue = 128;
            audioOutCount = 0;
            audioOutWaiting = false;
            audioOutNextSpkValue = 0;
            audioOutChange = 0;
            audioOutLatency = 0;
            WaveOutEvent waveOut = new WaveOutEvent
            {
                DesiredLatency = 75
            };
            waveOut.Init(this);
            Task.Run(waveOut.Play);
        }

        private void AudioOut_SpeakerChanger()
        {
            audioOutCount++;
            if (!audioOutWaiting)
            {
                ulong next;
                lock (audioOutEvents)
                {
                    if (audioOutEvents.Count == 0)
                        return;
                    next = audioOutEvents.Dequeue();
                }
                audioOutNextSpkValue = audioOutSpkLevels[next & 3];
                next >>= 2;
                audioOutChange = (ulong)((double)next / (double)cpuSpeed) + audioOutLatency;
                if (audioOutChange < audioOutCount)
                    audioOutLatency += 100;
                audioOutWaiting = true;
            }
            else
            {
                if (audioOutCount >= audioOutChange)
                {
                    audioOutSpkValue = audioOutNextSpkValue;
                    audioOutWaiting = false;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                AudioOut_SpeakerChanger();
                buffer[i] = audioOutSpkValue;
            }
            audioOutPosition += count;
            return count;
        }

    }
}

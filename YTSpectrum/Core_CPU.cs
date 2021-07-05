using Konamiman.Z80dotNet;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSpectrum
{
    class Core_CPU { }

    public partial class Core: IMemory, IZ80InterruptSource
    {
        private static readonly int cpuClockFreq = 3500000;


        private readonly Z80Processor cpu;
        private float cpuLastTCount;
        private readonly float cpuSpeed;
        private readonly byte[] cpuRam;
        private bool cpuIrq;

        public int Size => cpuRam.Length;

        public byte this[int address]
        {
            get => cpuRam[address];
            set => cpuRam[address] = value;
        }

        #pragma warning disable CS0067
        public event EventHandler NmiInterruptPulse;
        #pragma warning restore CS0067

        public bool IntLineIsActive
        {
            get
            {
                if (cpuIrq)
                {
                    cpuIrq = false;
                    return true;
                }
                return false;
            }
        }

        public byte? ValueOnDataBus => 255;

        private Core(Core_CPU _)
        {
            cpu = new Z80Processor();
            cpuIrq = false;
            cpuLastTCount = 0;
            cpuSpeed = (float)cpuClockFreq / (float)baseFreq;
            cpuRam = new byte[65536];
            Array.Copy(File.ReadAllBytes("48k.rom"), cpuRam, 16384);
            cpu.SetMemoryAccessMode(0, 16384, MemoryAccessMode.ReadOnly);            
            cpu.Memory = this;
            cpu.RegisterInterruptSource(this);
        }

        private void CPU_Execute_Block()
        {
            float tCount = cpuLastTCount;
            while (tCount < cpuSpeed)
                tCount += cpu.ExecuteNextInstruction();
            cpuLastTCount = tCount - cpuSpeed;
            Video_Scan();
        }

        private void CPU_Interrupt()
        {
            cpuIrq = true;
            Video_Display();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            Array.Copy(contents, 0, cpuRam, startIndex, length ?? contents.Length);
        }

        public byte[] GetContents(int startAddress, int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(cpuRam, startAddress, bytes, 0, length);
            return bytes;
        }

    }
}

using Konamiman.Z80dotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSpectrum
{
    class Core_IO : IMemory
    {
        public Action<int, byte> Set;
        public Func<int, byte> Get;
        public byte this[int address]
        {
            get => Get(address);
            set => Set(address, value);
        }
        public int Size => 65536;

        public byte[] GetContents(int startAddress, int length)
            => throw new NotImplementedException();

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
            => throw new NotImplementedException();
        
    }

    public partial class Core
    {
        private readonly Dictionary<Keys, int[]> ioKeyMap;
        private readonly Dictionary<int, IntBuffer> ioKeyState;
        private int ioSpeakerState;

        private Core(Core_IO portSpace) : this(new Core_CPU())
        {
            ioSpeakerState = 0;
            portSpace.Get = IO_PortIn;
            portSpace.Set = IO_PortOut;
            cpu.PortsSpace = portSpace;
            ioKeyMap = new Dictionary<Keys, int[]>
            {
                [Keys.ShiftKey] = new int[] { 0xfefe, 1 },
                [Keys.Z] = new int[] { 0xfefe, 2 },
                [Keys.X] = new int[] { 0xfefe, 4 },
                [Keys.C] = new int[] { 0xfefe, 8 },
                [Keys.V] = new int[] { 0xfefe, 16 },
                [Keys.A] = new int[] { 0xfdfe, 1 },
                [Keys.S] = new int[] { 0xfdfe, 2 },
                [Keys.D] = new int[] { 0xfdfe, 4 },
                [Keys.F] = new int[] { 0xfdfe, 8 },
                [Keys.G] = new int[] { 0xfdfe, 16 },
                [Keys.Q] = new int[] { 0xfbfe, 1 },
                [Keys.W] = new int[] { 0xfbfe, 2 },
                [Keys.E] = new int[] { 0xfbfe, 4 },
                [Keys.R] = new int[] { 0xfbfe, 8 },
                [Keys.T] = new int[] { 0xfbfe, 16 },
                [Keys.D1] = new int[] { 0xf7fe, 1 },
                [Keys.D2] = new int[] { 0xf7fe, 2 },
                [Keys.D3] = new int[] { 0xf7fe, 4 },
                [Keys.D4] = new int[] { 0xf7fe, 8 },
                [Keys.D5] = new int[] { 0xf7fe, 16 },
                [Keys.D0] = new int[] { 0xeffe, 1 },
                [Keys.D9] = new int[] { 0xeffe, 2 },
                [Keys.D8] = new int[] { 0xeffe, 4 },
                [Keys.D7] = new int[] { 0xeffe, 8 },
                [Keys.D6] = new int[] { 0xeffe, 16 },
                [Keys.P] = new int[] { 0xdffe, 1 },
                [Keys.O] = new int[] { 0xdffe, 2 },
                [Keys.I] = new int[] { 0xdffe, 4 },
                [Keys.U] = new int[] { 0xdffe, 8 },
                [Keys.Y] = new int[] { 0xdffe, 16 },
                [Keys.Enter] = new int[] { 0xbffe, 1 },
                [Keys.L] = new int[] { 0xbffe, 2 },
                [Keys.K] = new int[] { 0xbffe, 4 },
                [Keys.J] = new int[] { 0xbffe, 8 },
                [Keys.H] = new int[] { 0xbffe, 16 },
                [Keys.Space] = new int[] { 0x7ffe, 1 },
                [Keys.ControlKey] = new int[] { 0x7ffe, 2 },
                [Keys.M] = new int[] { 0x7ffe, 4 },
                [Keys.N] = new int[] { 0x7ffe, 8 },
                [Keys.B] = new int[] { 0x7ffe, 16 }
            };
            IntBuffer lastRow = new IntBuffer(0xbf);
            ioKeyState = new Dictionary<int, IntBuffer>
            {
                [0xfefe] = new IntBuffer(0xbf),
                [0xfdfe] = new IntBuffer(0xbf),
                [0xfbfe] = new IntBuffer(0xbf),
                [0xf7fe] = new IntBuffer(0xbf),
                [0xeffe] = new IntBuffer(0xbf),
                [0xdffe] = new IntBuffer(0xbf),
                [0xbffe] = new IntBuffer(0xbf),
                [0x7ffe] = lastRow,
                [0x00fe] = lastRow
            };
        }

        private byte IO_PortIn(int address)
        {
            if (ioKeyState.ContainsKey(address))
            {
                IntBuffer ks = ioKeyState[address];
                lock (ks)
                {
                    byte b = (byte)ks.Value;
                    if (audioInState) b |= 0x40;
                    return b;
                }
            }
            return 255;
        }

        private void IO_PortOut(int address, byte value)
        {
            if (address == 254)
            {
                videoBorderColor = (byte)(value & 7);
                int lastSpeaker = ioSpeakerState;
                ioSpeakerState = (value & 0x18) >> 3;
                if (ioSpeakerState != lastSpeaker)
                {
                    ulong sstate = (cpu.TStatesElapsedSinceStart << 2) | (uint)ioSpeakerState;
                    lock (audioOutEvents)
                        audioOutEvents.Enqueue(sstate);
                }
            }
        }

        public void IO_KeyPress(Keys key, bool down)
        {
            if (ioKeyMap.ContainsKey(key))
            {
                int port = ioKeyMap[key][0];
                int bit = ioKeyMap[key][1];
                IntBuffer ks = ioKeyState[port];
                lock (ks)
                {
                    int lv = ks.LastValue;
                    if (down)
                        lv &= ~bit & 0xff;
                    else
                        lv |= bit & 0xff;
                    ks.Value = lv;
                }
            }
        }

    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSpectrum
{
    class Core_Video 
    {
        public PictureBox screen;
        public Core_Video(PictureBox pBox) => screen = pBox;
    }

    public partial class Core
    {
        private static readonly Rectangle videoScreenRect = new Rectangle(0, 0, 416, 312);
        private static readonly int BRIGHT = 0xd7, NORM = 0xff;

        private float videoLastLine;
        private readonly float videoLineFreq;
        private int videoLineCount;
        private byte videoBorderColor;
        private bool videoFlashInvert;
        private readonly byte[] videoScreenData;
        private readonly Timer videoFlashTimer;
        private readonly PictureBox videoPBControl;
        private readonly Bitmap videoBitmap;
        private readonly MethodInvoker videoShow;

        private Core(Core_Video coreVideo) : this(new Core_AudioIn())
        {
            videoShow = Video_Show;
            videoLineFreq = (312f * 50f) / baseFreq;
            videoLineCount = 0;
            videoBorderColor = 0;
            videoFlashInvert = false;
            videoScreenData = new byte[312 * 416];
            videoFlashTimer = new Timer
            {
                Interval = 500
            };
            videoFlashTimer.Tick += Video_FlashTimerTick;
            videoFlashTimer.Start();
            videoBitmap = new Bitmap(416, 312, PixelFormat.Format8bppIndexed);
            ColorPalette pal = videoBitmap.Palette;
            pal.Entries[0] = Color.FromArgb(0, 0, 0);
            pal.Entries[1] = Color.FromArgb(0, 0, NORM);
            pal.Entries[2] = Color.FromArgb(NORM, 0, 0);
            pal.Entries[3] = Color.FromArgb(NORM, 0, NORM);
            pal.Entries[4] = Color.FromArgb(0, NORM, 0);
            pal.Entries[5] = Color.FromArgb(0, NORM, NORM);
            pal.Entries[6] = Color.FromArgb(NORM, NORM, 0);
            pal.Entries[7] = Color.FromArgb(NORM, NORM, NORM);
            pal.Entries[8] = Color.FromArgb(0, 0, 0);
            pal.Entries[9] = Color.FromArgb(0, 0, BRIGHT);
            pal.Entries[10] = Color.FromArgb(BRIGHT, 0, 0);
            pal.Entries[11] = Color.FromArgb(BRIGHT, 0, BRIGHT);
            pal.Entries[12] = Color.FromArgb(0, BRIGHT, 0);
            pal.Entries[13] = Color.FromArgb(0, BRIGHT, BRIGHT);
            pal.Entries[14] = Color.FromArgb(BRIGHT, BRIGHT, 0);
            pal.Entries[15] = Color.FromArgb(BRIGHT, BRIGHT, BRIGHT);
            videoBitmap.Palette = pal;
            videoPBControl = coreVideo.screen;
            videoPBControl.BackgroundImage = videoBitmap;
        }

        private void Video_FlashTimerTick(object sender, EventArgs e) => videoFlashInvert = !videoFlashInvert;

        private void Video_Scan()
        {
            videoLastLine += videoLineFreq;
            if (videoLastLine >= 1)
            {
                Video_DrawLine(videoLineCount++);
                if (videoLineCount >= 312)
                {
                    videoLineCount = 0;
                    CPU_Interrupt();
                }
                videoLastLine--;
            }
        }

        private void Video_DrawLine(int lineNo)
        {
            if (lineNo < 8) return;
            int lineStart = lineNo * 416;
            if (lineNo < 64 || lineNo >= 256) // top and bottom border
            {
                Fill(videoScreenData, videoBorderColor, lineStart, 416);
                return;
            }
            Fill(videoScreenData, videoBorderColor, lineStart, 80); // left border
            Fill(videoScreenData, videoBorderColor, lineStart + 336, 80); // right border
            lineStart += 80;
            lineNo -= 64;
            int charY = 0x5800 + ((lineNo >> 3) << 5);
            int lineAddr = ((lineNo & 0x07) << 8) | ((lineNo & 0x38) << 2) | ((lineNo & 0xC0) << 5) | 0x4000;
            for (int charX = 0; charX < 32; charX++)
            {
                byte att = cpuRam[charY + charX];
                int ink = att & 0x07;
                int paper = (att & 0x38) >> 3;
                if ((att & 0x40) != 0) { ink += 8; paper += 8; }
                bool flash = (att & 0x80) != 0;
                bool doFlash = flash && videoFlashInvert;
                byte byt = cpuRam[lineAddr++];
                for (int bit = 128; bit > 0; bit >>= 1)
                {
                    if (doFlash)
                        videoScreenData[lineStart++] = (byte)((byt & bit) != 0 ? paper : ink);
                    else
                        videoScreenData[lineStart++] = (byte)((byt & bit) != 0 ? ink : paper);
                }
            }
        }

        private void Video_Show()
        {
            byte[] clone = (byte[])videoScreenData.Clone();
            BitmapData bmd = videoBitmap.LockBits(
                    videoScreenRect,
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);
            Marshal.Copy(clone, 0, bmd.Scan0, clone.Length);
            videoBitmap.UnlockBits(bmd);
            videoPBControl.Refresh();
        }

        private void Video_Display()
        {
            if (!videoPBControl.IsDisposed)
                videoPBControl.Invoke(videoShow);
        }

        private static void Fill(byte[] array, byte with, int start, int len)
        {
            int end = start + len;
            while (start < end)
                array[start++] = with;
        }

    }

}

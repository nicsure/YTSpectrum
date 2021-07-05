using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTSpectrum
{
    public partial class UI : Form
    {
        private Core core;
        private readonly HashSet<Keys> keyStates;

        public UI()
        {
            InitializeComponent();
            CheckBox cb = new CheckBox
            {
                Size = Size.Empty
            };
            keyStates = new HashSet<Keys>();
            PB_Screen.Controls.Add(cb);
            PB_Screen.Click += PB_Screen_Click;
            cb.KeyDown += Cb_KeyDown;
            cb.KeyUp += Cb_KeyUp;
        }

        private void Cb_KeyUp(object sender, KeyEventArgs e)
        {
            keyStates.Remove(e.KeyCode);
            core?.IO_KeyPress(e.KeyCode, false);
            e.Handled = true;
        }

        private void Cb_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                    SpecialKey(Keys.ShiftKey, Keys.D0);
                    break;
                case Keys.Menu:
                    SpecialKey(Keys.ControlKey, Keys.ShiftKey);
                    break;
                case Keys.Up:
                    SpecialKey(null, Keys.D7);
                    break;
                case Keys.Down:
                    SpecialKey(null, Keys.D6);
                    break;
                case Keys.Left:
                    SpecialKey(null, Keys.D5);
                    break;
                case Keys.Right:
                    SpecialKey(null, Keys.D8);
                    break;
                case Keys.CapsLock:
                    SpecialKey(Keys.ShiftKey, Keys.D2);
                    break;
                default:
                    if (!keyStates.Contains(e.KeyCode))
                    {
                        keyStates.Add(e.KeyCode);
                        core?.IO_KeyPress(e.KeyCode, true);
                    }
                    break;
            }
            e.Handled = true;
        }

        private void SpecialKey(Keys? qualifier, Keys key)
        {
            KeyEventArgs qualArgs =
                qualifier != null && !keyStates.Contains((Keys)qualifier) ?
                new KeyEventArgs((Keys)qualifier) : null;
            KeyEventArgs keyArgs = new KeyEventArgs(key);
            if (qualArgs != null)
                Cb_KeyDown(this, qualArgs);
            Cb_KeyDown(this, keyArgs);
            Cb_KeyUp(this, keyArgs);
            if (qualArgs != null) 
                Cb_KeyUp(this, qualArgs);
        }

        private void PB_Screen_Click(object sender, EventArgs e) => PB_Screen.Controls[0].Focus();

        private void UI_Shown(object sender, EventArgs e) => core = new Core(PB_Screen);
        
    }
}

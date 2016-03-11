using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ThreadMan
{
    public partial class Form2 : Form
    {
        Work work;

        public Form2(Work work_)
        {
            work = work_;
            InitializeComponent();
        }

        // TODO: Останавливать таймер, когда он не нужен.
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (work.GetState() == Work.State.Aborted || work.GetState() == Work.State.Done)
                Close();

            var enabled = work.IsActive();
            progressBar1.SetState(enabled ? ProgressBarUtils.PBST_NORMAL : ProgressBarUtils.PBST_PAUSED);

            double p = work.GetProgress();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 65535;
            progressBar1.Value = (int)(p * 65535.0);


            btnToggle.Enabled = work.CanToggle();
            if (work.IsPaused())
                btnToggle.Text = "Возобновить";
            else
                btnToggle.Text = "Приостановить";

            label1.Text = work.ToString();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = work.GetName();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Close reason: {0}\n", e.CloseReason);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            work.Toggle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            work.Stop();
        }
    }

    public static class ProgressBarUtils
    {
        public const uint PBST_PAUSED = 0x0003;
        public const uint PBST_ERROR = 0x0002;
        public const uint PBST_NORMAL = 0x0001;

        public static void SetState(this ProgressBar pb, uint state)
        {


            if (!pb.IsDisposed)
            {
                SendMessage(pb.Handle,
        0x400 + 16, //WM_USER + PBM_SETSTATE
        state, //PBST_PAUSED
        0);
            }

            /*SendMessage(pb.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0002, //PBST_ERROR
              0);*/
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd,
          uint Msg,
          uint wParam,
          uint lParam);
    }
}

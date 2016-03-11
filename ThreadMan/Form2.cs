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

namespace ThreadMan
{
    public partial class Form2 : Form
    {
        Work work;

        public Form2( Work work_ )
        {
            work = work_;
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (work.GetState() == Work.State.Aborted || work.GetState() == Work.State.Done)
                Close();

            double p = work.GetProgress();
            if (!double.IsNaN(p))
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 65535;
                progressBar1.Value = (int)(p * 65535.0);
            }
            progressBar1.Enabled = work.IsActive() && !double.IsNaN(p);

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
}

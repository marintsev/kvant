using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace ThreadMan
{
    public partial class Form1 : Form
    {
        private List<Work> jobs = null;

        public Form1()
        {
            InitializeComponent();
            jobs = new List<Work>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var work = new Work();
            work.AddListener(() =>
            {
                BeginInvoke(new Action(() => { UpdateJobs(); }));
            });
            jobs.Add(work);
            UpdateJobs();
            work.Start();
            new Form2(work).Show();
        }

        private void CleanJobs()
        {
            var dead = new List<Work>();
            foreach (var l in jobs)
            {
                if (l.IsStopped())
                    dead.Add(l);
            }
            foreach (var l in dead)
            {
                jobs.Remove(l);
            }
            UpdateJobs();
        }

        private void UpdateUI()
        {
            btnTerminate.Enabled = HasActiveJobs();
            btnClean.Enabled = HasFinishedJobs();
            var selected = GetSelected();
            btnToggle.Enabled = selected != null && selected.CanToggle();
            btnAbort.Enabled = selected != null && !selected.IsStopped();
        }

        private void UpdateJobs()
        {
            listBox1.Items.Clear();
            foreach (var job in jobs)
            {
                listBox1.Items.Add(job.ToString());
            }
            UpdateUI();
        }

        private bool HasActiveJobs()
        {
            foreach (var job in jobs)
                if (job.IsActive())
                    return true;
            return false;
        }

        private bool HasFinishedJobs()
        {
            foreach (var job in jobs)
                if (job.IsStopped())
                    return true;
            return false;
        }

        private void TerminateJobs()
        {
            foreach (var job in jobs)
            {
                job.Stop();
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CleanJobs();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selected = GetSelected();
            if (selected == null)
            {
                if (listBox1.Items.Count != 0)
                    MessageBox.Show("Не выбрано задание.");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasActiveJobs())
            {
                MessageBox.Show( "Есть активные задания." );
                e.Cancel = true;
            }
        }

        private Work GetSelected()
        {
            var index = listBox1.SelectedIndex;
            if( index == -1 )
                return null;
            return jobs[index];
        }

        private void btnTerminate_Click(object sender, EventArgs e)
        {
            TerminateJobs();
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected != null)
                selected.Toggle();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected != null)
                selected.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void CarefulUpdateJobs()
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateJobs();
            //CarefulUpdateJobs();
        }
    }
}

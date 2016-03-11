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
            work.AddListener(() => {

                BeginInvoke(new Action(() => { UpdateJobs(); }));
            });
            jobs.Add(work);
            UpdateJobs();
            work.Start();
            new Form2(work).Show();
        }

        private void UpdateJobs()
        {
            listBox1.Items.Clear();
            foreach (var job in jobs)
            {
                listBox1.Items.Add(job.ToString());
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }
    }
}

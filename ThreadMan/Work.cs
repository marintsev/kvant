using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadMan
{
    public class Work
    {
        private Thread thread = null;
        private int counter;
        private int max_counter;

        public Work()
        {
            Create();
        }

        public void Create()
        {
            thread = new Thread(Function);
        }

        public void Start()
        {
            thread.Start();
        }

        public void Play()
        {
            thread.Resume();
        }

        public void Pause()
        {
            thread.Suspend();
        }

        public void Stop()
        {
            thread.Abort();
        }

        public double GetProgress()
        {
            return counter / (double)max_counter;
        }

        public void Function()
        {
            int i;
            double value = 123;
            max_counter = 1000000000;
            for (i = 0; i < max_counter; i++)
            {
                value = Math.Sin(value);
                counter = i;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ThreadMan
{
    public class Work
    {
        public delegate void Listener();
        public enum State { Allocated, Created, Running, Paused, Stopped, Aborted, Done };

        private State state_ = State.Allocated;
        private State state
        {
            get
            {
                return state_;
            }
            set
            {
                state_ = value;
                Notify();
            }
        }

        
        private Thread thread = null;
        private int counter;
        private int max_counter;
        static int Index = 1;
        int index;
        private List<Listener> auditory = null;
        public string Text = "";

        public Work()
        {
            index = Index++;
            auditory = new List<Listener>();
            Create();
        }

        public bool CanToggle()
        {
            return state == State.Running || state == State.Paused;
        }

        public bool IsPaused()
        {
            return state == State.Paused;
        }

        public void AddListener( Listener listener )
        {
            auditory.Add(listener);
        }

        public void Notify()
        {
            foreach( var l in auditory )
            {
                l();
            }
        }

        public void Create()
        {
            thread = new Thread(Function);
            thread.Priority = ThreadPriority.Lowest;
            state = State.Created;
        }

        public void Start()
        {
            thread.Start();
            state = State.Running;
        }

        public void Play()
        {
            thread.Resume();
            state = State.Running;
        }

        public void Pause()
        {
            thread.Suspend();
            state = State.Paused;
        }

        public void Toggle()
        {
            if (state == State.Running)
                Pause();
            else if (state == State.Paused)
                Play();
            else
                Debug.WriteLine("Work.Toggle with state {0}", state);
        }

        /// <summary>
        /// Выполняется.
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return !(IsStopped());
        }

        /// <summary>
        /// Выполняется или завершён.
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return state == State.Running || state == State.Done;
        }

        public void Stop()
        {
            if (state == State.Paused)
                Play();
            thread.Abort();
        }

        /// <summary>
        /// Задание было прервано или закончено?
        /// </summary>
        public bool IsStopped()
        {
            return state == State.Aborted || state == State.Done;
        }

        public double GetProgress()
        {
            var d = (double)counter / (double)max_counter;
            if (double.IsNaN(d))
                return 0.0;
            return d;
        }

        public string GetName()
        {
            return string.Format("Задание №{0}", index);
        }

        public override string ToString()
        {
            if( state == State.Running )
                Text = string.Format("{0} ({1} {2,3:0}%)", GetName(), state, GetProgress()*100.0);
            else
                Text = string.Format("{0} ({1})", GetName(), state);
            return Text;
        }

        public State GetState()
        {
            return state;
        }

        public void Function()
        {
            try
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
            catch (ThreadAbortException )
            {
                state = State.Aborted;
                return;
            }
            state = State.Done;
        }
    }
}

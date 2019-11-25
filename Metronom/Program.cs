using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Metronom
{
    public class Listener
    {
        public int ListID { set; get; }
        public Listener(int ID) { ListID = ID; }
        public void Subscribe(Metronome m)
        {
            m.Tick += HeardIt;
            m.Beep += BeepNow;
        }
        private void HeardIt(Metronome m, TimeOfTick e)
        {
            Console.WriteLine("{0} HEARD IT AT {1}",
            ListID, e.Time);
        }

        private void BeepNow(Metronome m, Beep b)
        {
            Console.WriteLine("\a");
        }
    }
    public class TimeOfTick : EventArgs
    {
        public DateTime Time { set; get; }
    }

    public class Beep : EventArgs
    {
        public int Freqency { set; get; }
    }
    public class Metronome
    {
        public event TickHandler Tick;
        public event BeepHandler Beep;
        public delegate void TickHandler(Metronome m, TimeOfTick e);
        public delegate void BeepHandler(Metronome m, Beep b);
        public void Check()
        {
            while (true)
            {
                    Thread.Sleep(2000);
                    TimeOfTick TOT = new TimeOfTick();
                    Beep bp = new Beep();
                    bp.Freqency = 420;
                    TOT.Time = DateTime.Now;
                    Tick?.Invoke(this, TOT);
                    Beep?.Invoke(this, bp);
                
            }
        }
    }
    class Program
    {
        static void Main()
        {
            Task<Metronome> t = new Task<Metronome>(() =>
            {
                Metronome m = new Metronome();
                m.Check();
                return m;
            });


            Task<Metronome> q = new Task<Metronome>(() =>
            {
                Metronome m = new Metronome();
                m.Check();
                return m;
            });

            t.Start();
            q.Start();

            Task.WaitAll(new Task[] { t, q });

            Listener l = new Listener(1);
            l.Subscribe(t.Result);
            Listener n = new Listener(2);
            n.Subscribe(q.Result);
        }
    }
}

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
            Console.Beep(b.Freqency, ListID*b.Duration);
        }
    }
    public class TimeOfTick : EventArgs
    {
        public DateTime Time { set; get; }
    }

    public class Beep : EventArgs
    {
        public int Freqency { set; get; }
        public int Duration { set; get; }
    }
    public class Metronome
    {
        public event TickHandler Tick;
        public event BeepHandler Beep;
        public delegate void TickHandler(Metronome m, TimeOfTick e);
        public delegate void BeepHandler(Metronome m, Beep b);
        public async Task Check()
        {
            await Task.Run( () =>
            {
                while (true)
                {
                    Thread.Sleep(2000);
                    TimeOfTick TOT = new TimeOfTick();
                    Beep bp = new Beep();
                    bp.Freqency = 420;
                    bp.Duration = 200;
                    TOT.Time = DateTime.Now;
                    Tick?.Invoke(this, TOT);
                    Beep?.Invoke(this, bp);
                }
            }
            );
        }
    }
    class Program
    {
        
        static void Main()
        {
            Metronome m = new Metronome();
            Metronome t = new Metronome();

            Listener l = new Listener(1);
            l.Subscribe(m);
            Listener n = new Listener(2);
            n.Subscribe(t);

            Task t1 = new Task(async () =>
            {
                await t.Check();
            });

            Task t2 = new Task(async () =>
            {
                await m.Check();
            });

            t1.Start();
            t2.Start();

            Console.WriteLine("Zpět v Main");
            Console.ReadLine();

           
        }
    }
}

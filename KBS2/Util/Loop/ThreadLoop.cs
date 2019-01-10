using System.Threading;
using System.Windows;
using CommandSystem.PropertyManagement;

namespace KBS2.Util.Loop
{
    public class ThreadLoop : TickLoop
    {
        private Thread thread;
        private bool running;
        private int interval;

        public ThreadLoop(string name, int tickRate = 30) : base(name, tickRate)
        {
            thread = new Thread(Run) {Name = Name};
            interval = CalculateInterval(TickRate);

            Application.Current.Exit += (sender, args) => Stop();
        }

        private void Run()
        {
            while (running)
            {
                Update(this, null);

                Thread.Sleep(interval);
            }
        }

        public override void Start()
        {
            if (!running)
            {
                while (thread.IsAlive)
                {
                    if (running)
                    {
                        return;
                    }

                    Thread.Sleep(20);
                }

                running = true;

                thread = new Thread(Run);
                thread.Start();
            }
        }

        public override void Stop()
        {
            running = false;
        }

        public override bool IsRunning()
        {
            return running;
        }

        protected override void OnTickrateChange(object source, UserPropertyChangedArgs args)
        {
            interval = CalculateInterval(args.ValueAfter);
        }
    }
}
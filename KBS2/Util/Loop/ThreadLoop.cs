using System;
using System.Threading;
using System.Windows;

namespace KBS2.Util.Loop
{
    public class ThreadLoop : TickLoop
    {
        private Thread thread;
        private bool running;
        private int interval;

        public ThreadLoop(string name) : base(name)
        {
            thread = new Thread(Run);
            interval = CalculateInterval(TickRate);

            // Because we're running our own thread, we need to make sure we properly
            // shut it down when the application exits to prevent lingering processes.
            try
            {
                // Try to bind our Stop function to the WPF application exit event.
                Application.Current.Exit += (sender, args) => Stop();
            }
            catch (Exception)
            {
                // If this fails we're in a non-WPF environment (probably a UnitTest)
                // Bind our Stop function to the C# ProcessExit event instead.
                AppDomain.CurrentDomain.ProcessExit += (sender, args) => Stop();
            }
        }

        /// <summary>
        /// Start function for the thread.
        /// creates a while loop that runs until this Loop is stopped.
        /// </summary>
        private void Run()
        {
            while (running)
            {
                Update(this, null);

                Thread.Sleep(interval);
            }
        }

        /// <summary>
        /// Start the loop
        /// </summary>
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

        /// <summary>
        /// Stop the loop
        /// </summary>
        public override void Stop()
        {
            running = false;
        }

        /// <summary>
        /// Returns true if loop is running.
        /// </summary>
        public override bool IsRunning()
        {
            return running;
        }

        /// <summary>
        /// Listen to when the TickRate gets changed and update the thread interval appropriately.
        /// </summary>
        protected override void OnTickrateChange(object source, CustomPropertyChangedArgs args)
        {
            interval = CalculateInterval(args.ValueAfter);
        }
    }
}
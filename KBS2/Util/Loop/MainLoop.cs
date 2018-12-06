using System;
using System.Windows.Threading;

namespace KBS2.Util.Loop
{
    public class MainLoop : TickLoop
    {
        private readonly DispatcherTimer timer;

        public MainLoop(string name) : base(name)
        {
            // Setup a DispatcherTimer to run on the main thread with the specified tickrate.
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(TickRate))
            };
            timer.Tick += Update;
        }

        /// <summary>
        /// Start the loop
        /// </summary>
        public override void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// Stop the loop
        /// </summary>
        public override void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// Returns true if loop is running.
        /// </summary>
        public override bool IsRunning()
        {
            return timer.IsEnabled;
        }

        /// <summary>
        /// Listen to when the TickRate gets changed and update the DispatcherTimer appropriately.
        /// </summary>
        protected override void OnTickrateChange(object source, CustomPropertyChangedArgs args)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(args.ValueAfter));
        }
    }
}

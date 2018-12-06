using System;
using System.Windows.Threading;
using CommandSystem.PropertyManagement;
using Math = System.Math;

namespace KBS2.Util.Loop
{
    public delegate void Update();

    public class MainLoop : TickLoop
    {
        private readonly DispatcherTimer timer;

        public MainLoop(string name) : base(name)
        {
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
        /// returns true if loop is running.
        /// </summary>
        /// <returns></returns>
        public override bool IsRunning()
        {
            return timer.IsEnabled;
        }

        /// <summary>
        /// Event if the tickrate changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected override void OnTickrateChange(object source, UserPropertyChangedArgs args)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(args.ValueAfter));
        }
    }
}

using System;
using System.Windows.Threading;
using KBS2.Utilities;
using Math = System.Math;

namespace KBS2
{
    public delegate void Update();

    public class MainLoop
    {
        private Property tickRate = new Property(30);
        public int TickRate {
            get => tickRate.Value;
            set => tickRate.Value = value;
        }

        private DispatcherTimer timer;

        private event Update UpdateEvent;

        public MainLoop()
        {
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(TickRate))
            };
            timer.Tick += Update;
        }

        public void Subscribe(Update subscriber)
        {
            UpdateEvent += subscriber;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void OnTickrateChange(object source, EventArgs args)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(TickRate));
        }

        private void Update(object source, EventArgs args)
        {
            UpdateEvent?.Invoke();
        }

        private static int CalculateInterval(int tickRate)
        {
            return (int) Math.Round(1000.0 / tickRate);
        }
    }
}

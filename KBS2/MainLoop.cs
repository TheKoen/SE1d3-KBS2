using System;
using System.Windows;
using System.Windows.Threading;
using KBS2.Console;
using KBS2.Util;
using Math = System.Math;

namespace KBS2
{
    public delegate void Update();

    public class MainLoop
    {
        private Property tickRate = new Property(30);
        public int TickRate => tickRate.Value;

        private DispatcherTimer timer;

        private event Update UpdateEvent;

        public MainLoop()
        {
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(TickRate))
            };
            timer.Tick += Update;
            tickRate.PropertyChanged += OnTickrateChange;
            CommandHandler.RegisterProperty("tickRate", ref tickRate);
        }

        public void Subscribe(Update subscriber)
        {
            UpdateEvent += subscriber;
        }

        public void Unsubscribe(Update subscriber)
        {
            UpdateEvent -= subscriber;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public bool IsRunning()
        {
            return timer.IsEnabled;
        }

        private void OnTickrateChange(object source, CustomPropertyChangedArgs args)
        {
            MainWindow.Console.Print($"Changing TickRate to {args.ValueAfter}Hz");
            timer.Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(args.ValueAfter));
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

using System;
using System.Windows.Media;
using System.Windows.Threading;
using KBS2.Console;
using KBS2.Util;
using Math = System.Math;

namespace KBS2
{
    public delegate void Update();

    public class MainLoop
    {
        private string Name { get; set; }
        
        private readonly Property tickRate = new Property(30);
        public int TickRate => tickRate.Value;

        private readonly DispatcherTimer timer;
        private int exceptionCount;

        private event Update UpdateEvent;

        public MainLoop(string name)
        {
            Name = name;
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, CalculateInterval(TickRate))
            };
            timer.Tick += Update;
            tickRate.PropertyChanged += OnTickrateChange;
            CommandHandler.RegisterProperty($"{Name}.tickRate", ref tickRate);
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
            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            try
            {
                UpdateEvent?.Invoke();
            }
            catch (Exception e)
            {
                MainWindow.Console.Print($"Exception in main loop: {e}", Colors.Red);

                exceptionCount++;
                if (exceptionCount > 2)
                {
                    Stop();
                    MainWindow.Console.Print("Main loop has been stopped due to too many exceptions!", Colors.Red);
                }
            }
            var taken = DateTimeOffset.Now.ToUnixTimeMilliseconds() - time;
            var interval = CalculateInterval(tickRate.Value);
            if (taken > interval)
            {
                MainWindow.Console.Print($"Main loop is running {taken - interval}ms behind!", Colors.Yellow);
            }
        }

        private static int CalculateInterval(int tickRate)
        {
            return (int) Math.Round(1000.0 / tickRate);
        }
    }
}

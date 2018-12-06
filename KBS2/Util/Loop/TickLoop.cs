﻿using System;
using System.Windows.Media;
using KBS2.Console;

namespace KBS2.Util.Loop
{
    public delegate void Update();

    public abstract class TickLoop
    {
        private string Name { get; }

        /// <summary>
        /// The tick-rate of this TickLoop.
        /// Determains how fast this loop has to run (in Hz)
        /// </summary>
        private readonly Property tickRate = new Property(30);
        public int TickRate => tickRate.Value;

        /// <summary>
        /// The Event that's called ever tick for this loop.
        /// </summary>
        private event Update UpdateEvent;

        /// <summary>
        /// The amount of exceptions that occurred in this loop.
        /// The loop automatically stops logging exceptions when
        /// this number is more than or equal to 3.
        /// </summary>
        private int exceptionCount;

        protected TickLoop(string name)
        {
            Name = name;
            tickRate.PropertyChanged += OnTickrateChange;
            CommandHandler.RegisterProperty($"{Name}.tickRate", ref tickRate);
        }

        /// <summary>
        /// subscribe to UpdateEvent
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(Update subscriber)
        {
            UpdateEvent += subscriber;
        }

        /// <summary>
        /// Unsubscribe to UpdateEvent
        /// </summary>
        /// <param name="subscriber"></param>
        public void Unsubscribe(Update subscriber)
        {
            UpdateEvent -= subscriber;
        }

        /// <summary>
        /// Start the loop
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stop the loop
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Returns true if loop is running.
        /// </summary>
        public abstract bool IsRunning();

        /// <summary>
        /// Event if the tickrate changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected abstract void OnTickrateChange(object source, CustomPropertyChangedArgs args);

        /// <summary>
        /// Called every Tick
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void Update(object source, EventArgs args)
        {
            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            try
            {
                UpdateEvent?.Invoke();
            }
            catch (Exception exception)
            {
                App.Console.Print($"Exception in main loop: {exception}", Colors.Red);

                exceptionCount++;
                if (exceptionCount > 2)
                {
                    Stop();
                    App.Console.Print("Main loop has been stopped due to too many exceptions!", Colors.Red);
                }
            }
            var taken = DateTimeOffset.Now.ToUnixTimeMilliseconds() - time;
            var interval = CalculateInterval(tickRate.Value);
            if (taken > interval)
            {
                App.Console.Print($"Main loop is running {taken - interval}ms behind!", Colors.Yellow);
            }
        }

        /// <summary>
        /// Calculates Interval of the Loop
        /// </summary>
        /// <param name="tickRate"></param>
        /// <returns></returns>
        protected static int CalculateInterval(int tickRate)
        {
            return (int)Math.Round(1000.0 / tickRate);
        }
    }
}

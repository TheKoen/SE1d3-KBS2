using System;
using System.Collections.Generic;
using System.Windows.Media;
using CommandSystem;
using CommandSystem.PropertyManagement;
using KBS2.Console;

namespace KBS2.Util.Loop
{
    public delegate void SyncAction();

    public abstract class TickLoop
    {
        protected string Name { get; }

        private Property tickRate = new Property(30);
        public int TickRate => tickRate.Value;

        private readonly Queue<SyncAction> Queue = new Queue<SyncAction>();

        private event Update UpdateEvent;

        private int exceptionCount;

        protected TickLoop(string name, int tickRate = 30)
        {
            Name = name;
            this.tickRate.Value = tickRate;
            this.tickRate.PropertyChanged += OnTickrateChange;
            Register();
        }

        public void Register()
        {
            try
            {
                PropertyHandler.RegisterProperty($"{Name}.tickRate", ref tickRate);
            }
            catch (Exception)
            {
                App.Console?.Print($"Unable to register tickRate property for {Name} loop", Colors.Yellow);
            }
        }

        public void EnqueueAction(SyncAction action)
        {
            Queue.Enqueue(action);
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
        /// returns true if loop is running.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsRunning();

        /// <summary>
        /// Event if the tickrate changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected abstract void OnTickrateChange(object source, UserPropertyChangedArgs args);

        /// <summary>
        /// Called every Tick
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void Update(object source, EventArgs args)
        {
            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            CatchExceptions(HandleQueue); 
            CatchExceptions(() => UpdateEvent?.Invoke());

            var taken = DateTimeOffset.Now.ToUnixTimeMilliseconds() - time;
            var interval = CalculateInterval(tickRate.Value);
            if (taken > interval)
            {
                App.Console.Print($"{Name} loop is running {taken - interval}ms behind!", Colors.Yellow);
            }
        }

        private void HandleQueue()
        {
            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            while (Queue.Count > 0)
            {
                Queue.Dequeue()?.Invoke();

                var taken = DateTimeOffset.Now.ToUnixTimeMilliseconds() - time;
                var interval = CalculateInterval(tickRate.Value);
                if (taken > interval) break;
            }
        }

        private void CatchExceptions(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception exception)
            {
                //throw exception;
                App.Console.Print($"Exception in {Name} loop: {exception}", Colors.Red);

                exceptionCount++;
                if (exceptionCount > 2)
                {
                    Stop();
                    App.Console.Print($"{Name} loop has been stopped due to too many exceptions!", Colors.Red);
                }
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

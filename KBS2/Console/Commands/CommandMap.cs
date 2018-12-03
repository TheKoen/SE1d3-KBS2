using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.GPS;
using KBS2.Util;

namespace KBS2.Console.Commands
{
    [CommandMetadata("map",
        Description = "Shows a downscaled representation of the current city",
        AutoRegister = true,
        Usages = new[] {"map [loop]"})]
    public class CommandMap : ICommand
    {
        private static bool running;
        private static bool active;

        static CommandMap()
        {
            active = true;
            var thread = new Thread(() =>
            {
                while (active)
                {
                    if (running)
                    {
                        PrintMap();
                    }

                    Thread.Sleep(400);
                }
            }) {Name = "DisplayMap Thread"};
            thread.Start();
        }

        public static void Stop()
        {
            active = false;
        }

        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length == 1 && args[0].Equals("loop"))
            {
                running = !running;
                return $"Turning map renderer {(running ? "on" : "off")}...";
            }

            PrintMap();
            return "Rendered map.";
        }

        private static void PrintMap()
        {
            var builder = new StringBuilder();
            builder.Append('\n');

            for (var y = 0; y < 450; y += 10)
            {
                for (var x = 0; x < 800; x += 10)
                {
                    var vector = new Vector(x, y);
                    if (GPSSystem.FindIntersection(vector) != null)
                    {
                        builder.Append('%');
                    }
                    else if (GPSSystem.GetRoad(vector) != null)
                    {
                        builder.Append('#');
                    }
                    else if (IsCustomer(vector))
                    {
                        builder.Append('$');
                    }
                    else if (IsBuilding(vector))
                    {
                        builder.Append('@');
                    }
                    else
                    {
                        builder.Append(' ');
                    }
                }

                builder.Append('\n');
            }

            App.Console.Print(builder.ToString());
        }

        private static bool IsCustomer(Vector point)
        {
            foreach (var customer in new List<Customer>(City.Instance.Customers))
            {
                if (MathUtil.Distance(customer.Location, point) < 10)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsBuilding(Vector point)
        {
            foreach (var building in new List<Building>(City.Instance.Buildings))
            {
                var location = building.Location;
                var size = building.Size / 2d;
                if (point.X > location.X - size && point.X < location.X + size &&
                    point.Y > location.Y - size && point.Y < location.Y + size)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
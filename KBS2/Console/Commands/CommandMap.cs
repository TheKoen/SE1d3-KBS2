using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBS2.CitySystem;
using KBS2.GPS;
using KBS2.Util;

namespace KBS2.Console.Commands
{
    [CommandMetadata("map",
        Description = "Shows a downscaled representation of the current city",
        AutoRegister = true)]
    public class CommandMap : ICommand
    {
        private static bool running;
        private static int secondTick;

        public IEnumerable<char> Run(params string[] args)
        {
            if (!running)
            {
                MainWindow.Loop.Subscribe(Update);
            }
            else
            {
                MainWindow.Loop.Unsubscribe(Update);
            }

            running = !running;

            return "Displaying map...";
        }

        private static void Update()
        {
            secondTick++;
            if (secondTick == 5)
            {
                secondTick = 0;
                PrintMap();
            }
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
                    if (GPSSystem.GetRoad(vector) != null)
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

            MainWindow.Console.Print(builder.ToString());
        }

        private static bool IsCustomer(Vector point)
        {
            foreach (var customer in City.Instance.Customers)
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
            foreach (var building in City.Instance.Buildings)
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
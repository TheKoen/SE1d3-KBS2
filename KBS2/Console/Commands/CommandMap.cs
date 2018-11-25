using System.Collections.Generic;
using System.Text;
using System.Windows;
using KBS2.CitySystem;
using KBS2.GPS;

namespace KBS2.Console.Commands
{
    [CommandMetadata("map",
        Description = "Shows a downscaled representation of the current city",
        AutoRegister = true)]
    public class CommandMap : ICommand
    {
        public IEnumerable<char> Run(params string[] args)
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

            return builder.ToString();
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
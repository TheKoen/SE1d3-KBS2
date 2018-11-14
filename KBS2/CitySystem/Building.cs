using System.Windows;

namespace KBS2.CitySystem
{
    public class Building
    {

        public Vector Location { get; set; }
        public int Size { get; set; }

        public Building(Vector l, int s)
        {
            this.Location = l;
            this.Size = s;
        }
    }
}

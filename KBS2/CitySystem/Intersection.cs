using System.Windows;

namespace KBS2.CitySystem
{
    public class Intersection
    {

        public Vector Location { get; set; }
        public int Size { get; set; }

        public Intersection(Vector l, int s)
        {
            this.Location = l;
            this.Size = s;
        }
    }
}

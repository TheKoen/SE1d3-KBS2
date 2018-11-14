using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2
{
    class Intersection
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

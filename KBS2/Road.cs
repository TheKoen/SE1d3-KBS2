using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2
{
    class Road
    {
        public Vector Start { get; set; }
        public Vector End { get; set; }
        public int Width { get; set; }
        public int MaxSpeed { get; set; }
    }
}

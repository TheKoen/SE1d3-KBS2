using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KBS2.Visual
{
    public class CustomerRenderHandler : IRenderHandler
    {
        public Canvas Canvas { get; }

        public CustomerRenderHandler(Canvas canvas)
        {
            Canvas = canvas;

            MainScreen.WPFLoop.Subscribe(Update);
        }

        public void Update()
        {
            
        }
    }
}

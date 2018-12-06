using KBS2.CitySystem;
using KBS2.CustomerSystem;
using KBS2.Visual.Controls;
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
            foreach(var child in Canvas.Children)
            {
                if(child is CustomerControl)
                {
                    ((CustomerControl)child).Update();
                }
            }
        }

        //nieuwe func die pakt alle customers in city en maakt controls
        private void AddControlForCustomersInCity(City city)
        {
            foreach(var customer in city.Customers)
            {
                Canvas.Children.Add(new CustomerControl(customer));
            }
        }
    }
}

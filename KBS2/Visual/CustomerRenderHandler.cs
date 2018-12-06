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
            if(City.Instance != null)
            {
                AddCostumerControls(City.Instance);
            }
        }

 
        private void AddCostumerControls(City city)
        {
            foreach(var customer in city.Customers)
            {
                if (!HasControlFor(customer))
                {
                    Canvas.Children.Add(new CustomerControl(customer));
                }
            }
        }

        public bool HasControlFor(Customer customer)
        {
            foreach (var child in Canvas.Children)
            {
                if (child is CustomerControl)
                {
                    var control = (CustomerControl)child;
                    if (control.customer.Equals(customer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

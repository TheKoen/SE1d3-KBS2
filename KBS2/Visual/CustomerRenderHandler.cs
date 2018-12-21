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
        public Canvas _Canvas { get; }
        public MainScreen Screen { get; set; }

        public CustomerRenderHandler(Canvas canvas, MainScreen screen)
        {
            _Canvas = canvas;
            Screen = screen;
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
                    var c = new CustomerControl(customer, Screen);
                    Panel.SetZIndex(c, 100);
                    _Canvas.Children.Add(c);
                }
            }
        }

        public bool HasControlFor(Customer customer)
        {
            foreach (var child in _Canvas.Children)
            {
                if (child is CustomerControl control)
                {
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

using KBS2.CarSystem;
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
    public class CarRenderHandler : IRenderHandler
    {
        public Canvas Canvas { get; }

        public CarRenderHandler(Canvas canvas)
        {
            Canvas = canvas;

            MainScreen.WPFLoop.Subscribe(Update);
        }

        public void Update()
        {
            if(City.Instance != null)
            {
                AddCarControls(City.Instance);
            }
        }

 
        private void AddCarControls(City city)
        {
            foreach(var car in city.Cars)
            {
                if (!HasControlFor(car))
                {
                    Canvas.Children.Add(new CarControl(car));
                }
            }
        }

        public bool HasControlFor(Car car)
        {
            foreach (var child in Canvas.Children)
            {
                if (child is CarControl)
                {
                    var control = (CarControl)child;
                    if (control.car.Equals(car))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

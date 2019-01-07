using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using KBS2.CarSystem;
using KBS2.CitySystem;
using KBS2.Visual.Controls;

namespace KBS2.CustomerSystem
{
    public class Customer : IEntity, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public double Moral { get; set; }
        public CustomerController Controller { get; set; }
        public int Age { get; set; }
        public Building Building { get; set; }
        public CustomerGroup Group { get; set; }
        public Moral Mood { get; set; }

        private Vector location;
        public Vector Location
        {
            get => location;
            set
            {
                location = value;
                LocationString = $"{Location.X:F0}, {Location.Y:F0}";
            }
        }

        private string locationString;

        public string LocationString
        {
            get => locationString;
            private set
            {
                if (locationString == value) return;

                locationString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationString"));
            }
        }

        
        public Building Destination
        {
            get => Group.Destination;
            set => Group.Destination = value;
        }

        public void UpdateDestination()
        {
            try
            {
                TargetLocationString = $"{Group.Destination.Location.X:F0}, {Group.Destination.Location.Y:F0}";
            }
            catch (Exception)
            {
                App.Console?.Print("Unable to update destination label!", Colors.Red);
            }
        }

        private string targetLocationString;

        public string TargetLocationString
        {
            get => targetLocationString;
            private set
            {
                if (targetLocationString == value) return;

                targetLocationString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TargetLocationString"));
            }
        }

        public Customer(Vector location, int age, Building building, CustomerGroup group)
        {
            Location = location;
            Age = age;
            Building = building;
            Moral = 25;
            Group = group;
            Mood = CustomerSystem.Moral.Happy;
            
            Controller = new CustomerController(this);
            MainScreen.AILoop.Subscribe(Controller.Update);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Vector GetLocation()
        {
            return Location;
        }

        public List<Vector> GetPoints()
        {
            return new List<Vector>
            {
                Location
            };

        }
    }
}

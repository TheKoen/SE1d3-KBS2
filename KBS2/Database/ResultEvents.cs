using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Database
{
    public class TripEventArgs : EventArgs
    {

        public CustomerSystem.CustomerGroup Group { get; }
        public System.Windows.Vector Start { get; }
        public System.Windows.Vector End { get; }
        public CarSystem.Car Car { get; }

        public TripEventArgs(CustomerSystem.CustomerGroup group, System.Windows.Vector start, System.Windows.Vector end, CarSystem.Car car)
        {
            Group = group;
            Start = start;
            End = end;
            Car = car;
        }
    }

    public class GroupEventArgs : EventArgs
    {

        public CustomerSystem.CustomerGroup Group { get; }
        public List<CustomerSystem.Customer> Customers { get; }

        public GroupEventArgs(CustomerSystem.CustomerGroup group, List<CustomerSystem.Customer> customers)
        {
            Group = group;
            Customers = customers;
        }
    }

    public class SimulationEventArgs : EventArgs
    {

        public string CityName { get; }
        public CitySystem.City City { get; }

        public SimulationEventArgs(CitySystem.City city)
        {
            CityName = city.Name;
            City = city;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using KBS2.CitySystem;
using Newtonsoft.Json;

namespace KBS2.CustomerSystem
{
    public class CustomerGroup
    {
        private static readonly Random Random = new Random();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public Building Destination { get; set; }
        public CustomerGroupController Controller { get; set; }
        public Vector Location { get; set; }
        public List<Road> RoadsNear;

        public CustomerGroup(int customers, Building start, Building destination)
        {
            for(var i = 0; i < customers; i++)
            {
                Customers.Add(new Customer(start.Location, Random.Next(4, 90), start, this));
            }
            Location = start.Location;
            Destination = destination;
            Controller = new CustomerGroupController(this);
            MainScreen.AILoop.Subscribe(Controller.Update);

            var thread = new Thread(() =>
            {
                var request = (HttpWebRequest) WebRequest.Create($"https://kbs.koenn.me/names.json");
                request.Method = "GET";
                request.ContentType = "application/json";

                if (!(request.GetResponse() is HttpWebResponse response))
                {
                    return;
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var data = reader.ReadToEnd();
                        var info = JsonConvert.DeserializeObject <CustomerInfo[]>(data);
                        var index = 0;
                        foreach (var customer in Customers)
                        {
                            customer.Name = $"{info[index].name} {info[index].surname}";
                            customer.Gender = info[index].gender;
                            index++;
                        }
                    }
                }
            });
            thread.Start();
        }

        private class CustomerInfo
        {
            public string name { get; set; }
            public string surname { get; set; }
            public string gender { get; set; }
            public string region { get; set; }
        }
    }
}

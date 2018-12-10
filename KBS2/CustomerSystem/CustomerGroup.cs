using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using KBS2.CitySystem;
using Newtonsoft.Json;
using Exception = System.Exception;

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

                HttpWebResponse response;
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                    if (response == null)
                    {
                        App.Console?.Print("Unable to request names for customers!", Colors.Red);
                        return;
                    }
                }
                catch (Exception)
                {
                    App.Console?.Print("Unable to connect to the server.", Colors.Red);
                    return;
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var info = JsonConvert.DeserializeObject <CustomerInfo[]>(reader.ReadToEnd());
                        var index = 0;
                        var rand = Random.Next(info.Length);
                        foreach (var customer in Customers)
                        {
                            customer.Name = $"{info[rand + index].name} {info[rand + index].surname}";
                            customer.Gender = info[rand + index].gender;
                            index++;
                        }
                    }
                    else
                    {
                        App.Console?.Print("Unable to parse names for customers!", Colors.Red);
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

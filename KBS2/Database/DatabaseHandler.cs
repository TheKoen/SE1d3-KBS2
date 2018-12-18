using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Database
{
    class DatabaseHandler
    {
        public void dbFunction()
        {
            using (var context = new MyDatabase("killakid"))
            {
                // Kijk of de cities tabel leeg is
                if (!context.Cities.Any())
                {
                    // Voeg een nieuwe city toe en save changes
                    context.Cities.Add(new Database.City { CityName = "BedenkWatLeuks" });
                    context.SaveChanges();
                }

                // Pak de eerste city uit de cities tabel
                var city = (from c in context.Cities
                            select c).ToList().First();

                // Make een nieuwe Garage om in de DB te stoppen.
                var garage = new Database.Garage
                {
                    // Zet de city naar de city die we uit de DB gehaald hadden
                    City = city,
                    // Zet de location naar een niewe vector
                    Location = new Database.Vector
                    {
                        X = 10
                    }
                };
                context.Garages.Add(garage);

                /*
                 * Je hoeft nergens ID's in te vullen, EntityFramework doet dit zelf.
                 * Let er wel op dat je Database.Vector en Database.City gebruikt, want
                 * City and Vector zijn ook al bestaande klassen in het project.
                 * Let ook op dat je alle velden leeg kan laten behalve de "name" van
                 * een city. Als je een int of double leeg laat wordt het default een 0
                 * en als je een string leeg laat wordt het default NULL.
                 */

                var inst = new Database.CityInstance
                {
                    City = city
                };

                var car = new Database.Car
                {
                    CityInstance = inst,
                    Garage = garage,
                    Model = "TestModel"
                };

                context.Cars.Add(car);

                var trip = new Database.Trip
                {
                    Car = car,
                    Distance = 100.3,
                    EndLocation = new Database.Vector
                    {
                        X = 984,
                        Y = 4385
                    },
                    StartLocation = new Database.Vector
                    {
                        X = 645,
                        Y = 235
                    },
                    Price = 100.5
                };

                context.Trips.Add(trip);

                context.SaveChanges();
            }
        }
    }
}

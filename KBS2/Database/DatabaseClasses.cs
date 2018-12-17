using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using MySql.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace KBS2.Database
{
    public class KillaKidConnection
    {
        private const string server = "62.4.27.251";
        private const string database = "killakid";
        private const string user = "root";
        private const string password = "C1rFMW8Wra";
        private const string port = "3306";
        private const string sslM = "none";

        private static readonly string connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", server, port, user, password, database, sslM);

        public static MySqlConnection OpenConnection()
        {
            var connection = new MySqlConnection(connectionString);

            connection.Open();

            return connection;
        }
    }

    public class myDatabase : DbContext
    {
        public myDatabase(string connectionstring) : base(connectionstring) { }
        public virtual DbSet<City> Cities { get; set; }
    }

    //public class Simulation {
    //    public int SimulationID { get; set; }
    //    public int Duration { get; set; }
    //    public int CityInstance_CityInstanceID { get; set; }
    //}

    //public class CityInstance
    //{
    //    public int CityInstanceID { get; set; }
    //    public string City_CityName { get; set; }
    //}

    public class City
    { 
        [Key]
        public string CityName { get; set; }
    }

    //public class Garage
    //{
    //    public int GarageID { get; set; }
    //    public int Vector_Location { get; set; }
    //    public string City_CityName { get; set; }
    //}

    //public class CustomerGroup
    //{
    //    public int CustomerGroupID { get; set; }
    //    public int Trip_TripID { get; set; }
    //    public int CityInstance_CityInstanceID { get; set; }
    //}

    //public class Car
    //{
    //    public int CarID { get; set; }
    //    public string Model { get; set; }
    //    public int Garage_GarageID { get; set; }
    //    public int CityInstance_CityInstanceID { get; set; }
    //}

    //public class Vector
    //{
    //    public int VectorID { get; set; }
    //    public double X { get; set; }
    //    public double Y { get; set; }
    //}

    //public class Trip
    //{
    //    public int TripID { get; set; }
    //    public int Vector_StartLocation { get; set; }
    //    public int Vector_EndLocation { get; set; }
    //    public int Car_CarID { get; set; }
    //    public double Price { get; set; }
    //    public double Distance { get; set; }
    //}

    //public class Gender
    //{
    //    public int GenderID { get; set; }
    //    public string Name { get; set; }
    //}

    //public class Customer
    //{
    //    public int CustomerID { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public int Age { get; set; }
    //    public int Gender_GenderID { get; set; }
    //    public int CustomerGroup_CustomerGroupID { get; set; }
    //}

    //public class Review
    //{
    //    public int ReviewID { get; set; }
    //    public string Content { get; set; }
    //    public int Rating { get; set; }
    //    public int Trip_TripID { get; set; }
    //    public int Customer_CustomerID { get; set; }
    //}


}

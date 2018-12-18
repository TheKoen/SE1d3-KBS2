using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace KBS2.Database
{
    public class MyDatabase : DbContext
    {
        public MyDatabase(string connectionstring) : base(connectionstring) { }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Simulation> Simulations { get; set; }
        public virtual DbSet<Garage> Garages { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

    }

    public class Simulation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Duration { get; set; }
        public virtual CityInstance CityInstance { get; set; }
    }

    public class CityInstance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual City City { get; set; }
    }

    public class City
    { 
        [Key]
        public string CityName { get; set; }
    }

    public class Garage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Vector Location { get; set; }
        public virtual City City { get; set; }
    }

    public class CustomerGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual CityInstance CityInstance { get; set; }
    }

    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Model { get; set; }
        public virtual Garage Garage { get; set; }
        public virtual CityInstance CityInstance { get; set; }
    }

    public class Vector
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Trip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Vector StartLocation { get; set; }
        public virtual Vector EndLocation { get; set; }
        public virtual Car Car { get; set; }
        public double Price { get; set; }
        public double Distance { get; set; }
    }

    public class Gender
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual CustomerGroup CustomerGroup { get; set; }
    }

    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Customer Customer { get; set; }
    }
}

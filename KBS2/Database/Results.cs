using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KBS2.Database
{
    class Results
    {
        public DatabaseHandler DatabaseHandler { get; private set; } = new DatabaseHandler();

        //lijst met info
        public List<Database.Customer> CustomerList { get; set; }

        //event based van simulatie de data opslaan, elke 5 seconde stuurt hij de data naar db.

        //public void OnCustomerGroupAdd(object source, EventArgs e)
        //{
        //    CustomerGroup group = e.Group;

        //    var dataGroup = new Database.CustomerGroup
        //    {
                
        //    }

        //    CustomerList.Add(new Database.Customer
        //    {
        //        FirstName = customer.FirstName,
        //        LastName = customer.LastName,
        //        Gender = customer.Gender,
        //        Age = customer.Age,
        //        CustomerGroup = customer.CustomerGroup
        //    });
        //}

        public void Setup()
        {
            DatabaseHandler.Setup();
        }

        public void Update()
        {

            DatabaseHandler.Update(this);
            
        }
    }

}

namespace KBS2.CustomerSystem
{
    public class CustomerController
    {
        public Customer Customer { get; set; }

        public CustomerController(Customer customer)
        {
            Customer = customer;

        }

        public void Update() { }
    }
}

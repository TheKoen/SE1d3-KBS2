using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KBS2.CustomerSystem
{
    class Review
    {
        public Customer CustomerData { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }

        public Review(Customer customer)
        {
            CustomerData = customer;
            Date = DateTime.Now;
            Rating = GiveRating(customer.Mood);
            Content = CreateContent(customer.Mood);
        }

        public string CreateContent(Moral moral)
        {
            var content = "";
            //Open xml
            var file = new XmlDocument();
            try
            {
                var path = new Uri(@" /KBS2;component/CustomerSystem/Reviews.xml", UriKind.Relative);
                file.Load(path.ToString());
            }
            catch
            {
                content = "Loading of content failed.";
            }

            var root = file.DocumentElement;
            if (root == null) throw new XmlException("Missing root node");

            var mood = file.SelectSingleNode($"//ReviewList/{moral}");
            if (mood == null)
                throw new XmlException("Missing mood in reviewlist.");

            Random random = new Random();

            content = mood.ChildNodes[random.Next(1, mood.ChildNodes.Count)].ToString();

            return content; 
            //Random description in section of depending moral
        }

        /// <summary>
        /// Give a rating depending on the <paramref name="moral"/> of the customer.
        /// </summary>
        /// <param name="moral"></param>
        /// <returns>a rating of type int</returns>
        public int GiveRating(Moral moral)
        {
            switch (moral)
            {
                case Moral.Happy:
                    return 5;
                case Moral.Neutral:
                    return 4;
                case Moral.Annoyed:
                    return 3;
                case Moral.Sad:
                    return 2;
                case Moral.Mad:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}

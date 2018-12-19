using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

            //Open XML File
            XDocument file = XDocument.Load(@"CustomerSystem/Reviews.xml");


            //Make a list of all reviews belonging to the current mood.
            List<string> reviewlist = new List<string>();

            //Query to fetch and fill the reviewlist above.
            try
            {
                reviewlist = (from r in file.Descendants("ReviewList").Elements($"{moral.ToString()}").Elements("review")
                              select r.Value).ToList();
            }
            catch (System.NullReferenceException)
            {
                return content = "Failed to load review content.";
            }

            //Random number for index will be generated and given as content
            Random random = new Random();



            return content = reviewlist[random.Next(0, reviewlist.Count)];
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

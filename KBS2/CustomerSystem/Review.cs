using System;
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
        /// <summary>
        /// Guide: https://www.codeproject.com/Articles/169598/%2FArticles%2F169598%2FParse-XML-Documents-by-XMLDocument-and-XDocument
        /// can open and read but now need to get the right values.
        /// </summary>
        /// <param name="moral"></param>
        /// <returns></returns>
        public string CreateContent(Moral moral)
        {
            var content = "";
            moral = Moral.Mad;
            XDocument file = XDocument.Load(@"CustomerSystem/Reviews.xml");

            try
            {
                var mood = file
                .Element("ReviewList")
                .Element($"{moral}");

            }
            catch (System.NullReferenceException)
            {
                return content = "Failed to load review content.";
            }




            Random random = new Random();



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

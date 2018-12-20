using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.Database
{
    public static class DatabaseHelper
    {
        private static readonly List<Gender> Genders = new List<Gender>();

        static DatabaseHelper()
        {
            using (var database = new MyDatabase("killakid"))
            {
                foreach (var gender in database.Genders)
                {
                    App.Console?.Print($"Loaded gender {gender.Name}");
                    Genders.Add(gender);
                }
            }
        }

        public static Gender GetGender(string gender)
        {
            var genderObject = Genders.Find(g => g.Name.Equals(gender));
            if (genderObject == null)
            {
                using (var database = new MyDatabase("killakid"))
                {
                    genderObject = new Gender
                    {
                        Name = gender
                    };
                    Genders.Add(genderObject);

                    database.Genders.Add(genderObject);
                    database.SaveChanges();
                }
            }

            return genderObject;
        }
    }
}

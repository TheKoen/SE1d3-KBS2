using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using KBS2.Util.Loop;

namespace KBS2.Database
{
    public delegate void DatabaseAction(MyDatabase database);
    public delegate void DatbaseCallback<T>(ICollection<T> data) where T : class;
    public delegate ICollection<T> DatabaseRequest<T>(MyDatabase database) where T : class;

    public static class DatabaseHelper
    {
        private static readonly List<Gender> Genders = new List<Gender>();
        private static readonly TickLoop DatabaseLoop = new ThreadLoop("DB");

        private static MyDatabase Database;

        static DatabaseHelper()
        {
            using (var database = new MyDatabase("killakid"))
            {
                foreach (var gender in database.Genders)
                {
                    App.Console?.Print($"Loaded gender {gender.Name}");
                    Genders.Add(gender);
                }

                Application.Current.Exit += (sender, args) => Database?.Dispose();
                Database = database;
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

        public static void QueueDatabaseAction(DatabaseAction action)
        {
            DatabaseLoop.EnqueueAction(() => action.Invoke(Database));
        }

        public static void QueueDatabaseRequest<T>(DatabaseRequest<T> request, DatbaseCallback<T> callback, TickLoop loop) where T : class
        {
            DatabaseLoop.EnqueueAction(() =>
            {
                var results = request.Invoke(Database);
                loop.EnqueueAction(() => callback.Invoke(results));
            });
        }

        public static Database.Vector CreateVector(System.Windows.Vector vector)
        {
            return new Vector
            {
                X = vector.X,
                Y = vector.Y
            };
        }

        public static T GetObject<T>(DbSet<T> set, Predicate<T> predicate) where T : class
        {
            var results = (from obj in set
                           where predicate(obj)
                           select obj).ToList();
            if (results.Count == 0)
            {
                return null;
            }
            return results.First();
        }

        public static bool MatchVectors(Vector vector1, System.Windows.Vector vector2)
        {
            return vector1.X == vector2.X && vector1.Y == vector2.Y;
        }
    }
}

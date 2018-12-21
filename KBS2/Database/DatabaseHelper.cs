using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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
            Application.Current.Exit += (sender, args) =>
            {
                DatabaseLoop.Stop();
                Database?.Dispose();
            };

            DatabaseLoop.EnqueueAction(() =>
            {
                App.Console?.Print("Connecting to database", Colors.LimeGreen);
                Database = new MyDatabase("killakid");
                App.Console?.Print("Connected!", Colors.LimeGreen);
            });

            QueueDatabaseAction((database) =>
            {
                App.Console?.Print($"Setting up DBHandler with DB {database}", Colors.LimeGreen);

                foreach (var gender in database.Genders)
                {
                    App.Console?.Print($"Loaded gender {gender.Name}");
                    Genders.Add(gender);
                }

            });
            DatabaseLoop.Start();
        }

        public static Gender GetGender(string gender)
        {
            var genderObject = Genders.Find(g => g.Name.Equals(gender));
            if (genderObject == null)
            {
                QueueDatabaseAction((database) =>
                {
                    genderObject = new Gender
                    {
                        Name = gender
                    };
                    Genders.Add(genderObject);

                    database.Genders.Add(genderObject);
                    database.SaveChanges();
                });
            }

            return genderObject;
        }

        public static void QueueDatabaseAction(DatabaseAction action)
        {
            App.Console?.Print($"Queued a DBAction", Colors.LimeGreen);
            DatabaseLoop.EnqueueAction(() => action.Invoke(Database));
        }

        public static void QueueDatabaseRequest<T>(DatabaseRequest<T> request, DatbaseCallback<T> callback, TickLoop loop) where T : class
        {
            App.Console?.Print($"Queued a DBRequest", Colors.LimeGreen);
            DatabaseLoop.EnqueueAction(() =>
            {
                var results = request.Invoke(Database);
                App.Console?.Print($"Retrieved {results.Count} items from DB, queueing callback", Colors.LimeGreen);
                loop.EnqueueAction(() => callback.Invoke(results));
            });
        }

        public static Vector CreateVector(System.Windows.Vector vector)
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
            return results.Count == 0 ? null : results.First();
        }

        public static bool MatchVectors(Vector vector1, System.Windows.Vector vector2)
        {
            return Math.Abs(vector1.X - vector2.X) < 0.01 && Math.Abs(vector1.Y - vector2.Y) < 0.01;
        }
    }
}

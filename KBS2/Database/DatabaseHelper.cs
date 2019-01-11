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
            /*
             * Hook into the application exit event to make sure
             * we dispose of the database correctly when exiting.
             */
            Application.Current.Exit += (sender, args) =>
            {
                DatabaseLoop.Stop();
                Database?.Dispose();
            };

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            // Queue an action to the DatabaseLoop to create the connection.
            DatabaseLoop.EnqueueAction(() =>
            {
                App.Console?.Print("Connecting to database", Colors.LimeGreen);
                Database = new MyDatabase("test");
                App.Console?.Print("Connected!", Colors.LimeGreen);
            });

            // Start the database loop.
            DatabaseLoop.Start();

            // Send a request to the database to retrieve and load all the genders.
            QueueDatabaseRequest(
                database => (from g in database.Genders
                    select g).ToList(),
                data =>
                {
                    foreach (var gender in data)
                    {
                        App.Console?.Print($"Loaded gender {gender.Name} from DB");
                        Genders.Add(gender);
                    }
                },
                MainScreen.CommandLoop
            );
        }

        /// <summary>
        /// Queues an action to the database. Generally used for saving
        /// data to the database.
        /// The DatabaseAction will always be executed on the database
        /// thread. It should be used to interact with the database.
        /// </summary>
        /// <param name="action">The action to execute on the database thread.</param>
        public static void QueueDatabaseAction(DatabaseAction action)
        {
            App.Console?.Print($"Queued a DBAction", Colors.LimeGreen);
            DatabaseLoop.EnqueueAction(() => action.Invoke(Database));
        }

        /// <summary>
        /// Queue a request for data to the database.
        /// The DatabaseRequest will always be executed on the database
        /// thread. It should be used to retrieve data from the database.
        /// The DatabaseCallback will be executed in the loop specified
        /// in the third argument. This can be used to process the data.
        /// </summary>
        /// <typeparam name="T">The type of data you want to request</typeparam>
        /// <param name="request">The request to execute on the database thread. Needs to return collection of data</param>
        /// <param name="callback">The callback to execute in the specified loop when the data has been retrieved</param>
        /// <param name="loop">The loop in which to call the callback</param>
        public static void QueueDatabaseRequest<T>(DatabaseRequest<T> request, DatbaseCallback<T> callback, TickLoop loop) where T : class
        {
            App.Console?.Print($"Queued a DBRequest", Colors.LimeGreen);
            DatabaseLoop.EnqueueAction(() =>
            {
                var results = request.Invoke(Database);
                int count;
                if (results != null)
                {
                    count = results.Count;
                    loop.EnqueueAction(() => callback.Invoke(results));
                }
                else
                {
                    count = 0;
                    loop.EnqueueAction(() => callback.Invoke(new List<T>()));
                }
                App.Console?.Print($"Retrieved {count} items from DB, queueing callback", Colors.LimeGreen);
            });
        }

        /// <summary>
        /// Turns a System.Windows.Vector into a Database.Vector to
        /// allow you to store it in the database.
        /// </summary>
        /// <param name="vector">System.Windows.Vector</param>
        /// <returns>Database.Vector</returns>
        public static Vector CreateDBVector(System.Windows.Vector vector)
        {
            return new Vector
            {
                X = vector.X,
                Y = vector.Y
            };
        }

        /// <summary>
        /// Get the gender object for the specified gender or
        /// create one if it doesn't exist.
        /// </summary>
        /// <param name="gender">Gender to get the object for</param>
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

                    App.Console?.Print($"Added gender {genderObject.Name} to DB");
                });
            }

            return genderObject;
        }
    }
}
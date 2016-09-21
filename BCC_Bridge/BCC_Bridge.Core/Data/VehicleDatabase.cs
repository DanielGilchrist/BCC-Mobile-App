using System;
using System.Collections.Generic;
using System.Linq;
using SQLite.Net;

namespace BCC_Bridge.Core
{
    public class VehicleDatabase
    {
        static object locker = new object();

        SQLiteConnection database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        public VehicleDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            // create the tables
            database.CreateTable<Vehicle>();
        }

        public IEnumerable<Vehicle> GetVehicles()
        {
            lock (locker)
            {
                return (from i in database.Table<Vehicle>() select i).ToList();
            }
        }

        //public IEnumerable<Vehicle> GetItemsNotDone()
        //{
        //	lock (locker)
        //	{
        //		return database.Query<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        //	}
        //}

        public Vehicle GetVehicle(int id)
        {
            lock (locker)
            {
                return database.Table<Vehicle>().FirstOrDefault(x => x.Id == id);
            }
        }

        public int SaveItem(Vehicle vehicle)
        {
            lock (locker)
            {
                if (vehicle.Id != 0)
                {
                    database.Update(vehicle);
                    return vehicle.Id;
                }
                else {
                    return database.Insert(vehicle);
                }
            }
        }

        public int DeleteVehicle(int id)
        {
            lock (locker)
            {
                return database.Delete<Vehicle>(id);
            }
        }
    }

}

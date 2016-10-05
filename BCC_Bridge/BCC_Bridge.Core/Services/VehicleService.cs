using MvxSqlite.Models;
using MvvmCross.Plugins.Sqlite;
using BCC_Bridge.Core;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace MvxSqlite.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly SQLiteConnection _connection;

        public VehicleService(IMvxSqliteConnectionFactory factory)
        {
            _connection = factory.GetConnection("Vehicles.sql");
            _connection.CreateTable<Vehicle>();
        }

        public List<Vehicle> All()
        {
            return _connection
                .Table<Vehicle>()
                //.OrderByDescending(x => x.WhenUtc)
                .ToList();
        }

        public void Add(Vehicle vehicle)
        {
            _connection.Insert(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
            _connection.Delete(vehicle);
        }

        public void Update(Vehicle vehicle)
        {
            _connection.Update(vehicle);
        }
    }
}
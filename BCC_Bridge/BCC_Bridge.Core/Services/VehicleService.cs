using MvvmCross.Plugins.Sqlite;
using BCC_Bridge.Core;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace MvxSqlite.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly SQLiteConnection _connection;

        public VehicleService(IMvxSqliteConnectionFactory _sqliteConnectionFactory)
        {
			Debug.WriteLine("VehicleService Constructor");

            _connection = _sqliteConnectionFactory.GetConnection("Vehicles.sqlite");
            _connection.CreateTable<Vehicle>();
        }

        public List<Vehicle> All()
		{
			Debug.WriteLine("VehicleService.All()");
            return _connection
                .Table<Vehicle>()
                //.OrderByDescending(x => x.WhenUtc)
                .ToList();
        }

        public void Add(Vehicle vehicle)
        {
			Debug.WriteLine("VehicleService.Add()");
            _connection.Insert(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
			Debug.WriteLine("VehicleService.Delete()");
            _connection.Delete(vehicle);
        }

        public void Update(Vehicle vehicle)
        {
			Debug.WriteLine("VehicleService.Update()");
            _connection.Update(vehicle);
        }
    }
}
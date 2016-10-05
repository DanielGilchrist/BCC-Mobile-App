using System.Collections.Generic;

namespace BCC_Bridge.Core
{
    public interface IVehicleService
    {
        List<Vehicle> All();
        void Add(Vehicle vehicle);
        void Delete(Vehicle vehicle);
        void Update(Vehicle vehicle)
    }
}
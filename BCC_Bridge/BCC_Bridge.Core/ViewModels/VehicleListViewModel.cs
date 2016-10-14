using System.Collections.Generic;
using System.Diagnostics;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
    public class VehicleListViewModel : MvxViewModel
    {
        private readonly IVehicleService _collectionService;

        public VehicleListViewModel(IVehicleService collectionService)
        {
            _collectionService = collectionService;
			var veh = new Vehicle() { Name = "test" };
            _collectionService.Add(veh);
            this.Vehicles = _collectionService.All();

			foreach (Vehicle vehicle in this.Vehicles)
			{
				Debug.WriteLine(vehicle);
			}

        }

        private List<Vehicle> _vehicles;
        public List<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set { _vehicles = value; RaisePropertyChanged(() => Vehicles); }
        }




        private MvvmCross.Core.ViewModels.MvxCommand<Vehicle> _itemSelectedCommand;
        public System.Windows.Input.ICommand ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand ?? new MvvmCross.Core.ViewModels.MvxCommand<Vehicle>(DoSelectItem);
                return _itemSelectedCommand;
            }
        }

        private void DoSelectItem(Vehicle vehicle)
        {
            //Console.WriteLine(vehicle);
        }
    }
}

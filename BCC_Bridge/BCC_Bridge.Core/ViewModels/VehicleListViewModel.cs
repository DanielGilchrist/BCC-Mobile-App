using System.Collections.Generic;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
    public class VehicleListViewModel : MvxViewModel
    {
        private readonly IVehicleService _collectionService;

        public VehicleListViewModel(IVehicleService collectionService)
        {
            _collectionService = collectionService;
            _collectionService.Add(new Vehicle() { Name = "test", Height = 120 });
            Vehicles = _collectionService.All();
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

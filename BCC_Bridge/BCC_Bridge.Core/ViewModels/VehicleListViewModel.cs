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
            this.Vehicles = _collectionService.All();
        }

		public void Refresh()
		{
			this.Vehicles = _collectionService.All();
		}

        private List<Vehicle> _vehicles;
        public List<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set { _vehicles = value; RaisePropertyChanged(() => Vehicles); }
        }

        private MvxCommand<Vehicle> _itemSelectedCommand;
        public System.Windows.Input.ICommand ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand ?? new MvxCommand<Vehicle>(DoSelectItem);
                return _itemSelectedCommand;
            }
        }

        private void DoSelectItem(Vehicle vehicle)
        {
			ShowViewModel<AddVehicleViewModel>(new AddVehicleParameters() { id = vehicle.Id - 1, editMode = true });
        }
    }
}

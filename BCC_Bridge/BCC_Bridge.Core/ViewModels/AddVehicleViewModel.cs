using System;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core
{
	public class AddVehicleViewModel : MvxViewModel
	{
		private Vehicle _vehicle;
		private readonly IVehicleService _collectionService;

		public AddVehicleViewModel(IVehicleService collectionService)
		{
			_collectionService = collectionService;
			this._vehicle = new Vehicle();
		}

		public double VehicleHeight
		{
			get { return _vehicle.Height; }
			set
			{
				_vehicle.Height = value;
				RaisePropertyChanged(() => VehicleHeight);
			}
		}

		public string VehicleName
		{
			get { return _vehicle.Name; }
			set
			{
				_vehicle.Name = value;
				RaisePropertyChanged(() => VehicleName);
			}
		}

		public IMvxCommand SaveVehicleButton { get { return _saveVehicle(); } }

		private MvxCommand _saveVehicle()
		{
			return new MvxCommand(() => _collectionService.Add(this._vehicle));
		}
	}
}

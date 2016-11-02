using System;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
	public class AddVehicleViewModel : MvxViewModel
	{
		private Vehicle _vehicle;
		private readonly IVehicleService _collectionService;
		public bool editing;

		public AddVehicleViewModel(IVehicleService collectionService)
		{
			editing = false;
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
			return new MvxCommand(() => {
				if (editing)
				{
					_collectionService.Update(this._vehicle);
				}
				else
				{
					_collectionService.Add(this._vehicle);
				}
			});
		}

		public void Init(AddVehicleParameters parameters)
		{
			if (parameters.editMode)
			{
				editing = true;
				this._vehicle = _collectionService.ById(parameters.id);
			}
		}
	}
}

using System;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core
{
	public class AddVehicleViewModel : MvxViewModel
	{
		public Vehicle Vehicle;

		private readonly IVehicleService _collectionService;

		public AddVehicleViewModel(IVehicleService collectionService)
		{
			_collectionService = collectionService;
			this.Vehicle = new Vehicle();
		}

		public void saveVehicle()
		{
			_collectionService.Add(this.Vehicle);
		}
	}
}

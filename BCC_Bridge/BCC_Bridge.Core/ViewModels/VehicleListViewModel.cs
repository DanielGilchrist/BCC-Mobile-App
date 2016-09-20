using System.Collections.Generic;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
	public class VehicleListViewModel : MvxViewModel
	{
		public override void Start()
		{
			// Initialise the vehicles list
			this.vehicles = new List<Vehicle> { };

			// Add a few dummy vehicles
			var test2 = new Vehicle() { name = "test", height = 200 };
			this.vehicles.Add(test2);
			this.vehicles.Add(test2);
			this.vehicles.Add(test2);
			this.vehicles.Add(test2);
			this.vehicles.Add(test2);
		}

		private List<Vehicle> _vehicles;
		public List<Vehicle> vehicles
		{
			get { return _vehicles; }
			set { _vehicles = value; RaisePropertyChanged(() => vehicles); }
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

using System.Collections.Generic;
using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
	public class VehicleListViewModel : MvxViewModel
	{
		public override void Start()
		{
			var test2 = new Vehicle() { name = "test 1" };
			// *************************************************************************************
			// This list.Add() Causes an exception to be thrown
			// *************************************************************************************
			//this.vehicles.Add(test2);
		}

		private List<Vehicle> _vehicles;
		// *************************************************************************************
		// Initialising the list with an item in it also throws an exception.
		// *************************************************************************************
		//public List<Vehicle> vehicles = new List<Vehicle> { new Vehicle() { name = "test" } };
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

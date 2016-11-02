using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace BCC_Bridge.Core.ViewModels
{
	public class MainViewModel : MvxViewModel
	{
		readonly Type[] _menuItemTypes = {
			typeof(MapViewModel),
			typeof(VehicleListViewModel),
			typeof(AddVehicleViewModel),
			typeof(HelpAboutViewModel),
		};

		public IEnumerable<string> MenuItems { get; private set; } = new[] {"Map", "Vehicles", "Add vehicle", "Help / About" };

		public void ShowDefaultMenuItem()
		{
			NavigateTo(0);
		}

		public void NavigateTo(int position)
		{
			ShowViewModel(_menuItemTypes[position]);
		}
	}

	public class AddVehicleParameters
	{
		public int id { get; set; }
		public bool editMode { get; set; }
	}

	public class MenuItem : Tuple<string, Type>
	{
		public MenuItem(string displayName, Type viewModelType)
			: base(displayName, viewModelType)
		{ }

		public string DisplayName
		{
			get { return Item1; }
		}

		public Type ViewModelType
		{
			get { return Item2; }
		}
	}
}
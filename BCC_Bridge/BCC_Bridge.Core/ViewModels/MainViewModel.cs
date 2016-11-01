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
		};

		public IEnumerable<string> MenuItems { get; private set; } = new[] { "Map", "Vehicles" };

		public void ShowDefaultMenuItem()
		{
			NavigateTo(0);
		}

		public void NavigateTo(int position)
		{
			ShowViewModel(_menuItemTypes[position]);
		}
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
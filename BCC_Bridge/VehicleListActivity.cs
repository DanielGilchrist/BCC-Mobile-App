using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace BCC_Bridge
{
	[Activity(Label = "BCC Bridge", MainLauncher = true)]
	public class VehicleListActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.VehicleListActivity);

			var vehicleList = new List<Vehicle> {
				new Vehicle() { name = "Big Mac", height = "3.5m" },
				new Vehicle() { name = "Tonka Truck", height = "2.2m" },
				new Vehicle() { name = "My Trailer", height = "2.8m" },
				new Vehicle() { name = "Car with bike strapped to top", height = "2.5m" }
			};

			var listView = FindViewById<ListView>(Resource.Id.ListView);
			listView.Adapter = new CustomListAdapter(this, vehicleList);
		}
	}
}



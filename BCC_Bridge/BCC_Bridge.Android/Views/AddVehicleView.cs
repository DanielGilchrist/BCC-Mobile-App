using Android.App;
using Android.OS;
using MvvmCross.Droid.Views;

namespace BCC_Bridge.Android.Views
{
	[Activity(Label = "View for AddVehicleViewModel")]
	public class AddVehicleView : MvxActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.AddVehicleView);
		}
	}
}


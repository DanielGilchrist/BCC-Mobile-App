using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

namespace BCC_Bridge
{
	[Activity(Label = "BCC Bridge", MainLauncher = true, Theme="@style/MyTheme")]
	public class MainActivity : ActionBarActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "Main" layout resource
			SetContentView (Resource.Layout.main);

            var mapButton = FindViewById<Button>(Resource.Id.btnMap);
            mapButton.Click += delegate { StartActivity(typeof(MapActivity)); };

			var menuButton = FindViewById<Button>(Resource.Id.btnMenu);
			menuButton.Click += delegate { StartActivity(typeof(MenuActivity)); };
		}
	}
}



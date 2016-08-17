using Android.App;
using Android.Widget;
using Android.OS;

namespace BCC_Bridge
{
	[Activity(Label = "BCC Bridge", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.YourMainView);


            var button = FindViewById<Button>(Resource.Id.button1);

            button.Click += delegate {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Yea boi");
                alert.SetMessage("ffgdgggd");

                var dialog = alert.Create();
                dialog.Show();
            };
		}
	}
}



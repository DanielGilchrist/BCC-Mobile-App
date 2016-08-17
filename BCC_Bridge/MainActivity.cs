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
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			SetContentView(Resource.Layout.Main);

			var tab = ActionBar.NewTab();
			tab.SetText("Tab 1");
			//tab.SetIcon(Resource.Drawable.tab1_icon);
			tab.TabSelected += (sender, args) =>
			{
				// Do something when tab is selected
			};

			ActionBar.AddTab(tab);

			tab = ActionBar.NewTab();
			tab.SetText("Tab 2");
			//tab.SetIcon(Resource.Drawable.tab2_icon);
			tab.TabSelected += (sender, args) =>
			{
				// Do something when tab is selected
			};

			ActionBar.AddTab(tab);
		}
	}
}



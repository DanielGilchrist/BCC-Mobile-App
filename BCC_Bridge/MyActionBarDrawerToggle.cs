using Android.Support.V4.Widget;
using Android.Support.V7.App;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace BCC_Bridge
{
	public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
	{
		private ActionBarActivity mHostActivity;
		private int mOpenedResource;
		private int mClosedResource;

		public MyActionBarDrawerToggle(ActionBarActivity host, DrawerLayout drawerLayout, int openedResource, int closedResource)
			: base(host, drawerLayout, openedResource, closedResource)
		{
			mHostActivity = host;
			mOpenedResource = openedResource;
			mClosedResource = closedResource;
		}

		public override void OnDrawerOpened(Android.Views.View drawerView)
		{
			base.OnDrawerOpened(drawerView);
			mHostActivity.SupportActionBar.SetTitle(mOpenedResource);
		}

		public override void OnDrawerClosed(Android.Views.View drawerView)
		{
			base.OnDrawerClosed(drawerView);
			mHostActivity.SupportActionBar.SetTitle(mClosedResource);
		}

		public override void OnDrawerSlide(Android.Views.View drawerView, float slideOffset)
		{
			base.OnDrawerSlide(drawerView, slideOffset);
		}
	}
}

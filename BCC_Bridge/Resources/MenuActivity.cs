
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace BCC_Bridge
{
	[Activity(Label = "MenuActivity", Theme = "@style/MyTheme")]
	public class MenuActivity : ActionBarActivity
	{
		private SupportToolbar mToolbar;
		private MyActionBarDrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;
		private ArrayAdapter mLeftAdapter;
		private List<string> mLeftDataSet;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Menu);

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

			mLeftDataSet = new List<string>();
			mLeftDataSet.Add("Help / About");
			mLeftDataSet.Add("Beacon");
			mLeftDataSet.Add("Alerts");
			mLeftDataSet.Add("Sounds");
			mLeftDataSet.Add("Background Idle");
			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;


			SetSupportActionBar(mToolbar);

			mDrawerToggle = new MyActionBarDrawerToggle(
				this,                       //host activity
				mDrawerLayout,        		//Drawer Layout
				Resource.String.openDrawer,	//Open message
				Resource.String.closeDrawer	//Closed message
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			//SupportActionBar.SetDisplayShowTitleEnabled(true);
			mDrawerToggle.SyncState();

			if (bundle != null)
			{
				if (bundle.GetString("DrawerState") == "Opened")
				{
					SupportActionBar.SetTitle(Resource.String.openDrawer);
				}
				else
				{
					SupportActionBar.SetTitle(Resource.String.closeDrawer);
				}

			}
			else
			{
				SupportActionBar.SetTitle(Resource.String.closeDrawer);
			}
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			mDrawerToggle.OnOptionsItemSelected(item);
			return base.OnOptionsItemSelected(item);
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
			{
				outState.PutString("DrawerState", "Opened");
			}
			else
			{
				outState.PutString("DrawerState", "Closed");
			}

			base.OnSaveInstanceState(outState);
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			mDrawerToggle.SyncState();
		}
	}
}


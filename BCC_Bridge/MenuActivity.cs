
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

namespace BCC_Bridge
{
	[Activity(Label = "Menu")]
	public class MenuActivity : Activity
	{
<<<<<<< HEAD
		private DrawerLayout m_Drawer;
		private ListView m_DrawerList;
		private static readonly string[] Sections = new[]
		{
			"Browse", "Friends", "Profile"
		};
=======
		private Button testBtn;
>>>>>>> 75c45ca68a22809eb47b721395420307d9c2593a

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

<<<<<<< HEAD
			SetContentView(Resource.Layout.Menu);

			this.m_Title = this.m_DrawerTitle = this.Title;

			this.m_Drawer = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			this.m_DrawerList = this.FindViewById<ListView>(Resource.Id.left_drawer);

			this.m_DrawerList.Adapter = new ArrayAdapter<string>(this, Resource.Layout.item_menu, Sections);
			this.m_DrawerList.ItemClick += DrawerListOnItemClick;

			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			this.m_DrawerToggle = new MyActionBarDrawerToggle(this, this.m_Drawer,
															  Resource.Drawable.ic_drawer_light,
															  Resource.String.drawer_open,
															  Resource.String.drawer_close);

			//Display the current fragments title and update the options menu
			this.m_DrawerToggle.DrawerClosed += delegate
			{
				this.ActionBar.Title = this.m_Title;
				this.InvalidateOptionsMenu();
			};

			//Display the drawer title and update the options menu
			this.m_DrawerToggle.DrawerOpened += delegate
			{
				this.ActionBar.Title = this.m_DrawerTitle;
				this.InvalidateOptionsMenu();
			};

			//Set the drawer listner to be the toggle

			this.m_Drawer.SetDrawerListener(this.m_DrawerToggle);

			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
			this.ActionBar.SetHomeButtonEnabled(true);
=======
            SetContentView(Resource.Layout.Menu); 

			testBtn = FindViewById<Button>(Resource.Id.btnTest);
			testBtn.Click += testBtn_Click;
>>>>>>> 75c45ca68a22809eb47b721395420307d9c2593a
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			this.m_DrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged(ViewConfiguration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			this.m_DrawerToggle.OnConfigurationChanged(newConfig);
		}

		//Pass the event to actionbardrawertoggle if it returns true
		//then it has handled the app icon touch event

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (this.mDrawerToggle.OnOptionsItemSelected(item))
				return true;

			return base.OnOptionsItemSelected(item);
		}

		private void DrawerListOnItemClick(Object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
		{
			Android.Support.V4.App.Fragment fragment = null;
			switch (itemClickEventArgs.Position)

			{
				case 0:
					fragment = new BrowseFragment();
					break;

				case 1:
					fragment = new FriendsFragment();
					break;
					
				case 2:
					fragment = new ProfileFragment();
					break;
			}

			SupportFragmentManager.BeginTransaction()
								  .Replace(Resource.Id.content_frame, fragment)
								  .Commit();

			this.m_DrawerList.SetItemChecked(itemClickEventArgs.Position, true);
			ActionBar.Title = this.m_Title = Sections[itemClickEventArgs.Position];
			this.m_Drawer.CloseDrawer(this.m_DrawerList);
		}

	}
}


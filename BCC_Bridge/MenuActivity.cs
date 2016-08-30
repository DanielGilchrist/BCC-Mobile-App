
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace BCC_Bridge
{
	[Activity(Label = "MenuActivity", Icon = "@drawable/icon", Theme ="@style/MyTheme")]
	public class MenuActivity : ActionBarActivity

	{
		private SupportToolbar mToolbar;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Menu);

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			SetSupportActionBar(mToolbar);
			// Create your application here
		}
	}
}

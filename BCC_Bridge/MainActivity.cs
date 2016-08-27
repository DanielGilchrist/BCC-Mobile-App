﻿using Android.App;
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
			SetContentView (Resource.Layout.main);

            var mapButton = FindViewById<Button>(Resource.Id.btnMap);
            mapButton.Click += delegate { StartActivity(typeof(MapActivity)); };
		}
	}
}



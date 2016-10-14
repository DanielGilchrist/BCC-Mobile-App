﻿using Android.App;
using Android.OS;
using MvvmCross.Droid.Views;

namespace BCC_Bridge.Android.Views
{
	[Activity(Label = "View for VehicleListViewModel")]
	public class VehicleListView : MvxActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.VehicleListView);
		}
	}
}


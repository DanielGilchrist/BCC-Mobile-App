
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BCC_Bridge
{
	[Activity(Label = "MenuActivity")]
	public class MenuActivity : Activity
	{
		private Button testBtn;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu); 

			testBtn = FindViewById<Button>(Resource.Id.btnTest);
			testBtn.Click += testBtn_Click;
		}
		void testBtn_Click(object sender, EventArgs e)
		{
			testBtn.Text = "Clicked!";
		}
	}
}


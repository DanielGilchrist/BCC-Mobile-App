
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

			testBtn = FindViewById<Button>(Resource.Id.btnTest);
			testBtn.Click += testBtn_Click;

			// Create your application here
		}
		void testBtn_Click(object sender, EventArgs e)
		{
			int count = 0;
			count++;
			testBtn.Text(count);
		}
	}
}


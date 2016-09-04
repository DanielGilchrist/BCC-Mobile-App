using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace BCC_Bridge
{
	public class CustomListAdapter : BaseAdapter<Vehicle>
	{
		Activity context;
		List<Vehicle> list;

		public CustomListAdapter(Activity _context, List<Vehicle> _list)
			: base()
		{
			this.context = _context;
			this.list = _list;
		}

		public override int Count
		{
			get { return list.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Vehicle this[int position]
		{
			get { return list[position]; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView;

			// re-use an existing view, if one is available
			// otherwise create a new one
			if (view == null)
				view = context.LayoutInflater.Inflate(Resource.Layout.VehicleListItemRow, parent, false);

			Vehicle item = this[position];
			view.FindViewById<TextView>(Resource.Id.Name).Text = item.name;
			view.FindViewById<TextView>(Resource.Id.Height).Text = item.height;

			return view;
		}
	}
}


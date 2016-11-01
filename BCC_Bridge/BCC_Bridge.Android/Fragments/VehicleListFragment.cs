using Android.OS;
using Android.Runtime;
using Android.Views;
using BCC_Bridge.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Binding.Droid.BindingContext;

namespace BCC_Bridge.Android
{
	[MvxFragmentAttribute(typeof(MainViewModel), Resource.Id.frameLayout)]
	[Register("bcc_bridge.android.VehicleListFragment")]
	public class VehicleListFragment : MvxFragment<VehicleListViewModel>
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var ignore = base.OnCreateView(inflater, container, savedInstanceState);
			return this.BindingInflate(Resource.Layout.VehicleListView, null);

			//return inflater.Inflate(Resource.Layout.VehicleListView, container, false);
		}
	}
}
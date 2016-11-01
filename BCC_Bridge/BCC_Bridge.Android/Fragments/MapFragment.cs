using Android.OS;
using Android.Runtime;
using Android.Views;
using BCC_Bridge.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Droid.Support.V4;

namespace BCC_Bridge.Android
{
	[MvxFragmentAttribute(typeof(MainViewModel), Resource.Id.frameLayout)]
	[Register("bcc_bridge.android.MapFragment")]
	public class MapFragment : MvxFragment<MapViewModel>
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.DummyMapView, container, false);
		}
	}
}
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Collections.Generic;
using Android.Graphics;

namespace BCC_Bridge
{
	[Activity(Label = "Map")]
	public class MapActivity : Activity, IOnMapReadyCallback
	{
		GoogleMap gMap;
		private int mapIndex = 1;
		private Button switchBtn;
		private EditText addressInput;
		private EditText vInput;
		private Marker marker = null;

		enum MarkerType
		{
			Normal = 1,
			Good = 2,
			Bad = 3
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.Map);

			string textColor = "#474342", hintColor = "#a99c98";

			switchBtn = FindViewById<Button>(Resource.Id.btnSwitch);
			switchBtn.Click += SwitchBtn_Click;

			addressInput = FindViewById<EditText>(Resource.Id.addressInput);
			addressInput.SetTextColor(Color.ParseColor(textColor));
			addressInput.SetHintTextColor(Color.ParseColor(hintColor));
			addressInput.EditorAction += Address_EditorAction;

			vInput = FindViewById<EditText>(Resource.Id.vehicleInput);
			vInput.SetTextColor(Color.ParseColor(textColor));
			vInput.SetHintTextColor(Color.ParseColor(hintColor));

			SetUpMap();
		}

		private void Address_EditorAction(object sender, EventArgs e)
		{
			SetCameraFromName(gMap, addressInput.Text);

			HideKeyboard(addressInput);
		}

		private void SwitchBtn_Click(object sender, EventArgs e)
		{
			mapIndex++;
			if (mapIndex > 4)
			{
				mapIndex = 1;
			}
			gMap.MapType = mapIndex;
		}

		private void HideKeyboard(EditText editText)
		{
			InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
			imm.HideSoftInputFromWindow(editText.WindowToken, 0);
		}

		private void SetUpMap()
		{
			if (gMap == null)
			{
				FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
			}
		}

		private void SetCameraFromCoords(GoogleMap map, double latitude, double longitude)
		{
			var camBuilder = new CameraPosition.Builder()
				.Target(new LatLng(latitude, longitude))
				.Zoom(16);

			var camPos = camBuilder.Build();
			var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);

			map.AnimateCamera(camUpdate);
		}

		private void SetCameraFromName(GoogleMap map, string name)
		{
			try
			{
				var geo = new Geocoder(this);
				var coords = geo.GetFromLocationName(name, 1);
				double latitude = coords[0].Latitude, longitude = coords[0].Longitude;

				SetCameraFromCoords(map, latitude, longitude);
				SetMarker(map, MarkerType.Normal, name, latitude, longitude, true);
			}
			catch
			{
				Toast.MakeText(this, "Invalid Location", ToastLength.Short).Show();
			}
		}

		private void SetMarker(GoogleMap map, MarkerType mt, string title, double latitude, double longitude, bool moveable)
		{
			var markerOptions = new MarkerOptions()
				.SetPosition(new LatLng(latitude, longitude))
				.SetTitle(title)
				.Draggable(moveable);

			if (mt == MarkerType.Good)
			{
				// markerOptions.SetIcon("good_marker");
			}
			else if (mt == MarkerType.Bad)
			{
				// markerOptions.SetIcon("bad_marker");
			}

			marker = map.AddMarker(markerOptions);
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			gMap = googleMap;

			SetCameraFromName(gMap, "Queensland University of Technology");
		}
	}
}
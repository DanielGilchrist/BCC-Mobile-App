using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Graphics;
using Android.Views.InputMethods;
using MvvmCross.Droid.Views;
using MvxSqlite.Services;
using BCC_Bridge.Core;
using BCC_Bridge.Core.ViewModels;
using BCC_Bridge.Core.Models;
using Android.Locations;

namespace BCC_Bridge.Android.Views
{
    [Activity(Label = "MapView")]
    public class MapView : MvxActivity, IOnMapReadyCallback
    {
        private delegate IOnMapReadyCallback OnMapReadyCallback();
        private GoogleMap gMap;
        MapViewModel mapViewModel;
        BridgeService bridgeService;
        List<Bridge> bridges;
        private int mapIndex = 1;
        private Button switchBtn;
        private Button locationBtn;
        private EditText addressInput;
        private EditText vInput;
        private Marker marker = null;

        enum MarkerType
        {
            Normal = 1,
            Good = 2,
            Bad = 3
        }

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("InnerException\n{0}", e.InnerException));
            }

            SetContentView(Resource.Layout.MapView);

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

            bridgeService = new BridgeService();
            bridges = bridgeService.All();

            SetUpMap();
        }

        private void SetUpMap()
        {
            mapViewModel = ViewModel as MapViewModel;
            if (gMap == null)
            {
                var mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map) as MapFragment;
                mapFragment.GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mapViewModel.OnMapSetup(SetMyLocation, SetMyLocationMarker);
            gMap = googleMap;
            gMap.SetPadding(0, 114, 0, 0);
            gMap.MyLocationEnabled = true;
            gMap.MyLocationChange += Map_MyLocationChange;

            /*MarkerType type;
            for (int i = 0; i < bridges.Count; i++)
            {

                if (bridges[i].Signed_Clearance < 4.0)
                {
                    type = MarkerType.Bad;
                } else
                {
                    type = MarkerType.Good;
                }

                SetMarker(gMap, type, bridges[i].Signed_Clearance.ToString(), bridges[i].Latitude, bridges[i].Longitude, false);
            }*/
        }

        private void Map_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            gMap.MyLocationChange -= Map_MyLocationChange;
            var location = new GeoLocation(e.Location.Latitude, e.Location.Longitude);
            SetMyLocation(location);
            mapViewModel.OnMyLocationChanged(location);
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

        private void SetMyLocation(GeoLocation geoLocation, float zoom = 18)
        {
            CameraPosition.Builder camBuilder = CameraPosition.InvokeBuilder();
            camBuilder.Target(new LatLng(geoLocation.Latitude, geoLocation.Longitude));
            camBuilder.Zoom(zoom);

            var cameraPosition = camBuilder.Build();
            var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            gMap.AnimateCamera(cameraUpdate);
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
            }
            catch
            {
                Toast.MakeText(this, "Invalid Location", ToastLength.Short).Show();
            }
        }

        private void SetMyLocationMarker(GeoLocation location)
        {
            /*var markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(location.Latitude, location.Longitude));
            markerOptions.SetTitle(location.Locality);
            gMap.AddMarker(markerOptions);*/
        }

        private void SetMarker(MarkerType mt, string title, double latitude, double longitude, bool moveable)
        {
            var markerOptions = new MarkerOptions()
                .SetPosition(new LatLng(latitude, longitude))
                .SetTitle(title)
                .Draggable(moveable);

            if (mt == MarkerType.Good)
            {
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.good_marker));
            }
            else if (mt == MarkerType.Bad)
            {
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.bad_marker));
            }

            marker = gMap.AddMarker(markerOptions);
        }
    }
}
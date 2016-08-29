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

namespace BCC_Bridge
{
    [Activity(Label = "Map")]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        GoogleMap gMap;
        private int mapIndex = 1;
        private Button switchBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Map);

            switchBtn = FindViewById<Button>(Resource.Id.btnSwitch);
            switchBtn.Click += SwitchBtn_Click;

            SetUpMap();
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

        private void SetUpMap()
        {
            if (gMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        private void SetCameraFromCoords(ref GoogleMap map, double latitude, double longitude)
        {
            var location = new LatLng(latitude, longitude);
            var camBuilder = new CameraPosition.Builder();

            camBuilder.Target(location);
            camBuilder.Zoom(16);

            var camPos = camBuilder.Build();
            var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);

            map.MoveCamera(camUpdate);
        }

        private void SetCameraFromName(ref GoogleMap map, string name)
        {
            // hacky solution to GetFromLocationName() timeout bug
            try
            {
                var geo = new Geocoder(this);
                var coords = geo.GetFromLocationName(name, 1);
                double latitude = coords[0].Latitude, longitude = coords[0].Longitude;

                SetCameraFromCoords(ref map, latitude, longitude);
                SetMarker(ref map, name, latitude, longitude);
            } 
            catch { /* don't do anything fam */ }
        }

        private void SetMarker(ref GoogleMap map, string title, double latitude, double longitude)
        {
            var marker = new MarkerOptions()
                .SetPosition(new LatLng(latitude, longitude))
                .SetTitle(title);

            map.AddMarker(marker);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            gMap = googleMap;

            SetCameraFromName(ref gMap, "Queensland University of Technology");
        }
    }
}
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
        private EditText address;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Map);

            //switchBtn = FindViewById<Button>(Resource.Id.btnSwitch);
            //switchBtn.Click += SwitchBtn_Click;

            address = FindViewById<EditText>(Resource.Id.addressInput);
            address.SetTextColor(Color.ParseColor("#474342"));
            address.SetHintTextColor(Color.ParseColor("#a99c98"));

            address.EditorAction += Address_EditorAction;

            SetUpMap();
        }

        private void Address_EditorAction(object sender, EventArgs e)
        {
            SetCameraFromName(ref gMap, address.Text);
            
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
            var camBuilder = new CameraPosition.Builder()
                .Target(new LatLng(latitude, longitude))
                .Zoom(16);

            var camPos = camBuilder.Build();
            var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);

            map.MoveCamera(camUpdate);
        }

        private void SetCameraFromName(ref GoogleMap map, string name)
        {
            // hacky (hopefully) temporary solution for GetFromLocationName() timeout bug
            try
            {
                var geo = new Geocoder(this);
                var coords = geo.GetFromLocationName(name, 1);
                double latitude = coords[0].Latitude, longitude = coords[0].Longitude;

                SetCameraFromCoords(ref map, latitude, longitude);
                SetMarker(ref map, name, latitude, longitude, true);
            } 
            catch { /* don't do anything fam */ }
        }

        private void SetMarker(ref GoogleMap map, string title, double latitude, double longitude, bool moveable)
        {
            var marker = new MarkerOptions()
                .SetPosition(new LatLng(latitude, longitude))
                .SetTitle(title);

            marker.Draggable(moveable);

            map.AddMarker(marker);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            gMap = googleMap;

            SetCameraFromName(ref gMap, "Queensland University of Technology");
        }
    }
}
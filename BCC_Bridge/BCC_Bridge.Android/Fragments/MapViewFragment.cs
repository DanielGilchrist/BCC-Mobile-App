using Android.OS;
using Android.Runtime;
using Android.Views;
using BCC_Bridge.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Droid.Support.V4;
using System;
using System.Threading;
using Android.App;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Graphics;
using Android.Views.InputMethods;
using MvxSqlite.Services;
using BCC_Bridge.Core;
using BCC_Bridge.Core.Models;
using Android.Locations;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using BCC_Bridge.Android.Maps;
using MvvmCross.Binding.Droid.BindingContext;


namespace BCC_Bridge.Android
{
	[MvxFragmentAttribute(typeof(MainViewModel), Resource.Id.frameLayout)]
	[Register("bcc_bridge.android.MapViewFragment")]
	public class MapViewFragment : MvxFragment<MapViewModel>, IOnMapReadyCallback
	{
        private delegate IOnMapReadyCallback OnMapReadyCallback();
        private GoogleMap gMap;
        MapViewModel mapViewModel;
        BridgeService bridgeService;
        List<Bridge> bridges;
        List<Bridge> badBridges;
        private int mapIndex = 1;
        private Button switchBtn;
        private EditText addressInput;
        private EditText vInput;
        private Marker marker = null;
        private double vehicleHeight;
        private bool placingBridgeMarkers;
        GeoLocation myGeoLocation;
        WebClient webclient;
        LatLng origin;
        LatLng destination;

        enum MarkerType
        {
            Normal = 1,
            Good = 2,
            Bad = 3
        }

		// Inflate the view associated with this fragment
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var ignore = base.OnCreateView(inflater, container, savedInstanceState);
			View v = this.BindingInflate(Resource.Layout.MapView, null);

			// Maybe initialise all of this somewhere else? not sure if it will cause hangs as
			// the fragment will wait until OnCreateView has returned a View.
			string textColor = "#474342", hintColor = "#a99c98";

			switchBtn = v.FindViewById<Button>(Resource.Id.btnSwitch);
			switchBtn.Click += SwitchBtn_Click;

            addressInput = v.FindViewById<EditText>(Resource.Id.addressInput);
            addressInput.SetTextColor(Color.ParseColor(textColor));
            addressInput.SetHintTextColor(Color.ParseColor(hintColor));
            addressInput.EditorAction += Address_EditorAction;

            vInput = v.FindViewById<EditText>(Resource.Id.vehicleInput);
            vInput.SetTextColor(Color.ParseColor(textColor));
            vInput.SetHintTextColor(Color.ParseColor(hintColor));
            vInput.EditorAction += VehicleInput_EditorAction;
            vehicleHeight = 4;
            placingBridgeMarkers = false;

            bridgeService = new BridgeService();
            bridges = bridgeService.All();
            badBridges = new List<Bridge>();

            SetUpMap();

            // OnCreateView must return the View
            return v;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();
			var f = this.Activity.FragmentManager.FindFragmentById(Resource.Id.map) as MapFragment;
			if (f != null)
				this.Activity.FragmentManager.BeginTransaction().Remove(f).Commit();
		}

        private void SetUpMap()
		{
            mapViewModel = ViewModel as MapViewModel;
            if (gMap == null)
            {
                var mapFragment = this.Activity.FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map) as MapFragment;
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

            ThreadPool.QueueUserWorkItem(o => SetBridgeMarkers(bridges, vehicleHeight));
        }

        private void Map_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            gMap.MyLocationChange -= Map_MyLocationChange;
            myGeoLocation = new GeoLocation(e.Location.Latitude, e.Location.Longitude);
            SetMyLocation(myGeoLocation);
            mapViewModel.OnMyLocationChanged(myGeoLocation);
        }

        private void Address_EditorAction(object sender, EventArgs e)
        {
            var destCoords = GetCoordsFromName(addressInput.Text);
            double destLat = destCoords[0].Latitude, destLong = destCoords[0].Longitude;

            destination = new LatLng(destLat, destLong);
            ProcessRoute();

            HideKeyboard(addressInput);
        }

        private void VehicleInput_EditorAction(object sender, EventArgs e)
        {
            HideKeyboard(vInput);

            if (vInput.Text != "")
            {
                vehicleHeight = double.Parse(vInput.Text);

                if (placingBridgeMarkers == true)
                {
                    Toast.MakeText(this.Activity, "Please wait for bridge markers to be placed before entering a new value", ToastLength.Short).Show();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(o => SetBridgeMarkers(bridges, vehicleHeight));
                }
            }
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
			InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(global::Android.Content.Context.InputMethodService);
            imm.HideSoftInputFromWindow(editText.WindowToken, 0);
        }

        private void SetMyLocation(GeoLocation geoLocation, float zoom = 18)
        {
            CameraPosition.Builder camBuilder = CameraPosition.InvokeBuilder();
            camBuilder.Target(GetMyLocation());
            camBuilder.Zoom(zoom);

            var cameraPosition = camBuilder.Build();
            var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            gMap.AnimateCamera(cameraUpdate);
        }

        private LatLng GetMyLocation()
        {
            return new LatLng(myGeoLocation.Latitude, myGeoLocation.Longitude);
        }

        private void SetCameraFromCoords(GoogleMap map, double latitude, double longitude, float zoom = 16)
        {
            var camBuilder = new CameraPosition.Builder()
                .Target(new LatLng(latitude, longitude))
                .Zoom(zoom);

            var camPos = camBuilder.Build();
            var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);

            map.AnimateCamera(camUpdate);
        }

        private void SetCameraFromName(GoogleMap map, string name)
        {
            try
            {
                var coords = GetCoordsFromName(name);
                double latitude = coords[0].Latitude, longitude = coords[0].Longitude;

                SetCameraFromCoords(map, latitude, longitude);
            }
            catch
            {
				Toast.MakeText(this.Activity, "Invalid Location", ToastLength.Short).Show();
            }
        }

        private IList<Address> GetCoordsFromName(string name)
        {
			var geo = new Geocoder(this.Activity);
            var coords = geo.GetFromLocationName(name, 1);

            return coords;
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

        private void SetBridgeMarkers(List<Bridge> bridges, double height)
        {
            this.Activity.RunOnUiThread(() => gMap.Clear());

            this.Activity.RunOnUiThread(() => Toast.MakeText(this.Activity, "Loading Bridge Markers...", ToastLength.Short).Show());
            placingBridgeMarkers = true;

            badBridges.Clear();

            MarkerType type;
            for (int i = 0; i < bridges.Count - 1; i++)
            {
                if (bridges[i].Signed_Clearance <= height)
                {
                    type = MarkerType.Bad;
                    badBridges.Add(bridges[i]);
                }
                else
                {
                    type = MarkerType.Good;
                }

                Thread.Sleep(50); // doesn't work without this... 
                this.Activity.RunOnUiThread(() => SetMarker(type, bridges[i].Signed_Clearance.ToString(), bridges[i].Latitude, bridges[i].Longitude, false));
            }

            placingBridgeMarkers = false;
            this.Activity.RunOnUiThread(() => Toast.MakeText(this.Activity, "Bridge Markers Loaded", ToastLength.Short).Show());

            if (destination != null)
            {
                this.Activity.RunOnUiThread(() => ProcessRoute());
            }
        }

        public string MakeDirectionURL(double originLatitude, double originLongitude, double destLatitude, double destLongitude)
        {
            StringBuilder url = new StringBuilder();

            url.Append("http://route.cit.api.here.com/routing/7.2/calculateroute.json");
            url.Append("?app_id=YueFlTt5s8iXeXb0VZPx");
            url.Append("&app_code=sG2iAqVYSywf0KjhpK1drA");
            url.AppendFormat("&waypoint0=geo!{0},{1}", originLatitude, originLongitude);
            url.AppendFormat("&waypoint1=geo!{0},{1}", destLatitude, destLongitude);
            url.Append("&mode=fastest;truck;traffic:disabled");
            url.Append("&avoidareas=");

            var boundsBuilder = new LatLngBounds.Builder();
            boundsBuilder.Include(origin);
            boundsBuilder.Include(destination);
            var bounds = boundsBuilder.Build();

            var badBridgesOnRoute = new List<Bridge>();

            for (int i = 0; i < badBridges.Count; i++)
            {
                if (bounds.Contains(new LatLng(badBridges[i].Latitude, badBridges[i].Longitude)))
                {
                    badBridgesOnRoute.Add(badBridges[i]);
                }
            }

            Console.WriteLine("badBridgesOnRoute: " + badBridgesOnRoute.Count);

            double variant = 0.001;
            for (int i = 0; i < badBridgesOnRoute.Count; i++)
            {
                if (badBridgesOnRoute.Count < 20)
                {
                    url.AppendFormat("{0},{1};{2},{3}!", badBridgesOnRoute[i].Latitude + variant, badBridgesOnRoute[i].Longitude + variant,
                                                     badBridgesOnRoute[i].Latitude - variant, badBridgesOnRoute[i].Longitude - variant);
                }
            }
            url.Length--;

            return url.ToString();
        }

        private void ProcessRoute()
        {
            var oAddress = GetMyLocation();
            var dAddress = destination;

            double oLat = oAddress.Latitude, oLong = oAddress.Longitude;
            double dLat = dAddress.Latitude, dLong = dAddress.Longitude;

            origin = new LatLng(oLat, oLong);
            destination = new LatLng(dLat, dLong);

            SetCameraFromCoords(gMap, origin.Latitude, origin.Longitude);

            if (origin != null && destination != null)
            {
                DrawRoute();
            }
        }

        private async void DrawRoute()
        {
            string url = MakeDirectionURL(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);
            string DirectionJSONResponse = await DirectionHttpRequest(url);

            this.Activity.RunOnUiThread(() => {
                SetMarker(MarkerType.Normal, "Origin", origin.Latitude, origin.Longitude, false);
                SetMarker(MarkerType.Normal, "Destination", destination.Latitude, destination.Longitude, false);
            });

            SetDirectionQuery(DirectionJSONResponse);
        }

        private void SetDirectionQuery(string response)
        {
            var routesObject = JsonConvert.DeserializeObject<HereJSONResponse>(response).response;

            Console.WriteLine("Number of routes: " + routesObject.route.Count);

            if (routesObject.route.Count > 0)
            {
                var routes = routesObject.route;
                var points = new LatLng[routes[0].leg[0].maneuver.Count];

                for (int i = 0; i < routes[0].leg[0].maneuver.Count; i++)
                {
                    points[i] = new LatLng(routes[0].leg[0].maneuver[i].position.latitude, routes[0].leg[0].maneuver[i].position.longitude);
                }

                var polyOption = new PolylineOptions();
                polyOption.InvokeColor(Color.Yellow);
                polyOption.InvokeWidth(5);
                polyOption.Add(points);

                this.Activity.RunOnUiThread(() => gMap.AddPolyline(polyOption));
            }
            else
            {
                Toast.MakeText(this.Activity, "No route returned", ToastLength.Long).Show();
            }
        }

        async Task<string> DirectionHttpRequest(string url)
        {
            webclient = new WebClient();

            string result;

            try
            {
                result = await webclient.DownloadStringTaskAsync(new Uri(url));
            }
            catch
            {
                Toast.MakeText(this.Activity, "Unable to draw route. Please try again.", ToastLength.Short);
                result = "";
            }
            finally
            {
                if (webclient != null)
                {
                    webclient.Dispose();
                    webclient = null;
                }
            }

            return result;
        }
    }
}
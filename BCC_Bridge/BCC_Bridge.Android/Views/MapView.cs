//using System;
//using System.Threading;
//using Android.App;
//using Android.Widget;
//using Android.OS;
//using Android.Gms.Maps;
//using Android.Gms.Maps.Model;
//using System.Collections.Generic;
//using Android.Graphics;
//using Android.Views.InputMethods;
//using MvvmCross.Droid.Views;
//using MvxSqlite.Services;
//using BCC_Bridge.Core;
//using BCC_Bridge.Core.ViewModels;
//using BCC_Bridge.Core.Models;
//using Android.Locations;
//using System.Net;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Text;
//using System.Linq;
//using BCC_Bridge.Android.Maps;
//using Android.Support.V4.Content;
//using Android;
//using Android.Content.PM;

//namespace BCC_Bridge.Android.Views
//{
//    [Activity(Label = "MapView")]
//    public class MapView : MvxActivity, IOnMapReadyCallback
//    {
//        private delegate IOnMapReadyCallback OnMapReadyCallback();
//        private GoogleMap gMap;
//        MapViewModel mapViewModel;
//        BridgeService bridgeService;
//        List<Bridge> bridges;
//        private int mapIndex = 1;
//        private Button switchBtn;
//        private EditText addressInput;
//        private EditText vInput;
//        private Marker marker = null;
//        private double vehicleHeight;
//        private bool placingBridgeMarkers;
//        GeoLocation myGeoLocation;
//        WebClient webclient;
//        LatLng origin;
//        LatLng destination;

//        enum MarkerType
//        {
//            Normal = 1,
//            Good = 2,
//            Bad = 3
//        }

//        protected override void OnCreate(Bundle bundle)
//        {
//            base.OnCreate(bundle);

//            SetContentView(Resource.Layout.MapView);

//            string textColor = "#474342", hintColor = "#a99c98";

//            switchBtn = FindViewById<Button>(Resource.Id.btnSwitch);
//            switchBtn.Click += SwitchBtn_Click;

//            addressInput = FindViewById<EditText>(Resource.Id.addressInput);
//            addressInput.SetTextColor(Color.ParseColor(textColor));
//            addressInput.SetHintTextColor(Color.ParseColor(hintColor));
//            addressInput.EditorAction += Address_EditorAction;

//            vInput = FindViewById<EditText>(Resource.Id.vehicleInput);
//            vInput.SetTextColor(Color.ParseColor(textColor));
//            vInput.SetHintTextColor(Color.ParseColor(hintColor));
//            vInput.EditorAction += VehicleInput_EditorAction;
//            vehicleHeight = 4;
//            placingBridgeMarkers = false;

//            bridgeService = new BridgeService();
//            bridges = bridgeService.All();

//            SetUpMap();
//        }

//        private void SetUpMap()
//        {
//            mapViewModel = ViewModel as MapViewModel;
//            if (gMap == null)
//            {
//                //var mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map) as MapFragment;
//                //mapFragment.GetMapAsync(this);
//            }
//        }

//        public void OnMapReady(GoogleMap googleMap)
//        {
//            mapViewModel.OnMapSetup(SetMyLocation, SetMyLocationMarker);
//            gMap = googleMap;
//            gMap.SetPadding(0, 114, 0, 0);
//            gMap.MyLocationEnabled = true;
//            gMap.MyLocationChange += Map_MyLocationChange;

//            try
//            {
//                ProcessRoute(); // PLEASE FUCKING WORK
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Process Route Exception: " + e.ToString());
//            }

//            //ThreadPool.QueueUserWorkItem(o => SetBridgeMarkers(bridges, vehicleHeight));
//        }

//        private void Map_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
//        {
//            gMap.MyLocationChange -= Map_MyLocationChange;
//            myGeoLocation = new GeoLocation(e.Location.Latitude, e.Location.Longitude);
//            SetMyLocation(myGeoLocation);
//            mapViewModel.OnMyLocationChanged(myGeoLocation);
//        }

//        private void Address_EditorAction(object sender, EventArgs e)
//        {
//            SetCameraFromName(gMap, addressInput.Text);

//            HideKeyboard(addressInput);
//        }

//        private void VehicleInput_EditorAction(object sender, EventArgs e)
//        {
//            HideKeyboard(vInput);

//            if (vInput.Text != "")
//            {
//                vehicleHeight = double.Parse(vInput.Text);

//                if (placingBridgeMarkers == true)
//                {
//                    Toast.MakeText(this, "Please wait for bridge markers to be placed before entering a new value", ToastLength.Short).Show();
//                }
//                else
//                {
//                    gMap.Clear();
//                    ThreadPool.QueueUserWorkItem(o => SetBridgeMarkers(bridges, vehicleHeight));
//                }
//            }
//        }

//        private void SwitchBtn_Click(object sender, EventArgs e)
//        {
//            mapIndex++;
//            if (mapIndex > 4)
//            {
//                mapIndex = 1;
//            }
//            gMap.MapType = mapIndex;
//        }

//        private void HideKeyboard(EditText editText)
//        {
//            InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
//            imm.HideSoftInputFromWindow(editText.WindowToken, 0);
//        }

//        private void SetMyLocation(GeoLocation geoLocation, float zoom = 18)
//        {
//            CameraPosition.Builder camBuilder = CameraPosition.InvokeBuilder();
//            camBuilder.Target(GetMyLocation());
//            camBuilder.Zoom(zoom);

//            var cameraPosition = camBuilder.Build();
//            var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

//            gMap.AnimateCamera(cameraUpdate);
//        }

//        private LatLng GetMyLocation()
//        {
//            return new LatLng(myGeoLocation.Latitude, myGeoLocation.Longitude);
//        }

//        private void SetCameraFromCoords(GoogleMap map, double latitude, double longitude, float zoom = 16)
//        {
//            var camBuilder = new CameraPosition.Builder()
//                .Target(new LatLng(latitude, longitude))
//                .Zoom(zoom);

//            var camPos = camBuilder.Build();
//            var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);

//            map.AnimateCamera(camUpdate);
//        }

//        private void SetCameraFromName(GoogleMap map, string name)
//        {
//            try
//            {
//                var coords = GetCoordsFromName(name);
//                double latitude = coords[0].Latitude, longitude = coords[0].Longitude;
                
//                SetCameraFromCoords(map, latitude, longitude);
//            }
//            catch
//            {
//                Toast.MakeText(this, "Invalid Location", ToastLength.Short).Show();
//            }
//        }

//        private IList<Address> GetCoordsFromName(string name)
//        {
//            var geo = new Geocoder(this);
//            var coords = geo.GetFromLocationName(name, 1);

//            return coords;
//        }

//        private void SetMyLocationMarker(GeoLocation location)
//        {
//            /*var markerOptions = new MarkerOptions();
//            markerOptions.SetPosition(new LatLng(location.Latitude, location.Longitude));
//            markerOptions.SetTitle(location.Locality);
//            gMap.AddMarker(markerOptions);*/
//        }

//        private void SetMarker(MarkerType mt, string title, double latitude, double longitude, bool moveable)
//        {
//            var markerOptions = new MarkerOptions()
//                .SetPosition(new LatLng(latitude, longitude))
//                .SetTitle(title)
//                .Draggable(moveable);

//            if (mt == MarkerType.Good)
//            {
//                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.good_marker));
//            }
//            else if (mt == MarkerType.Bad)
//            {
//                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.bad_marker));
//            }

//            marker = gMap.AddMarker(markerOptions);
//        }

//        private void SetBridgeMarkers(List<Bridge> bridges, double height)
//        {
//            RunOnUiThread(() => Toast.MakeText(this, "Loading Bridge Markers...", ToastLength.Short).Show());
//            placingBridgeMarkers = true;

//            MarkerType type;
//            for (int i = 0; i < bridges.Count - 1; i++)
//            {
//                if (bridges[i].Signed_Clearance <= height)
//                {
//                    type = MarkerType.Bad;
//                }
//                else
//                {
//                    type = MarkerType.Good;
//                }

//                Thread.Sleep(10); // doesn't work without this... 
//                RunOnUiThread(() => SetMarker(type, bridges[i].Signed_Clearance.ToString(), bridges[i].Latitude, bridges[i].Longitude, false));
//            }

//            placingBridgeMarkers = false;
//            RunOnUiThread(() => Toast.MakeText(this, "Bridge Markers Loaded", ToastLength.Short).Show());
//        }

//        public string MakeDirectionURL(double originLatitude, double originLongitude, double destLatitude, double destLongitude)
//        {
//            StringBuilder url = new StringBuilder();
//            url.Append("https://maps.googleapis.com/maps/api/directions/json");
//            url.Append("?origin=");// from
//            url.Append(originLatitude);
//            url.Append(",");
//            url.Append(originLongitude);
//            url.Append("&destination=");// to
//            url.Append(destLatitude);
//            url.Append(",");
//            url.Append(destLongitude);
//            url.Append("&mode=driving&alternatives=true");
//            url.Append("&key=AIzaSyAtYVVEVhHpesj31u0VVRBjwUzC6Z25lms");

//            Console.WriteLine(string.Format("DESINATION URL: {0}", url.ToString()));

//            return url.ToString();
//        }

//        private void ProcessRoute()
//        {
//            var oAddress = GetCoordsFromName("Queensland University of Technology");
//            var dAddress = GetCoordsFromName("University of Queensland");

//            double oLat = oAddress[0].Latitude, oLong = oAddress[0].Longitude;
//            double dLat = dAddress[0].Latitude, dLong = dAddress[0].Longitude;

//            origin = new LatLng(oLat, oLong);
//            destination = new LatLng(dLat, dLong);

//            SetCameraFromCoords(gMap, origin.Latitude, origin.Longitude);

//            if (origin != null && destination != null)
//            {
//                DrawPath();
//            }
//        }

//        private async void DrawPath()
//        {
//            string url = MakeDirectionURL(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);
//            string DirectionJSONResponse = await DirectionHttpRequest(url);

//            Console.WriteLine("DirectionJSONResponse:\n" + DirectionJSONResponse);

//            RunOnUiThread(() => {
//                gMap.Clear();
//                SetMarker(MarkerType.Normal, "Origin", origin.Latitude, origin.Longitude, false);
//                SetMarker(MarkerType.Normal, "Destination", destination.Latitude, destination.Longitude, false);
//            });

//            SetDirectionQuery(DirectionJSONResponse);
//        }

//        private void SetDirectionQuery(string response)
//        {
//            var routesObject = JsonConvert.DeserializeObject<GoogleDirection>(response);

//            if (routesObject.routes.Count > 0)
//            {
//                string encodedPoints = routesObject.routes[0].overview_polyline.points;
//                var decodedPoints = DecodePolyPoints(encodedPoints);

//                var points = new LatLng[decodedPoints.Count];
//                int i = 0;
//                foreach(LatLng location in decodedPoints)
//                {
//                    points[i++] = new LatLng(location.Latitude, location.Longitude);
//                }

//                var polyOption = new PolylineOptions();
//                polyOption.InvokeColor(Color.Red);
//                polyOption.InvokeWidth(10);
//                polyOption.Geodesic(true);
//                polyOption.Visible(true);
//                polyOption.Add(points);

//                RunOnUiThread(() => gMap.AddPolyline(polyOption));
//            }
//        }

//        private List<LatLng> DecodePolyPoints(string encodedPoints)
//        {
//            if (string.IsNullOrEmpty(encodedPoints)) { return null; }

//            var poly = new List<LatLng>();
//            char[] polyChars = encodedPoints.ToCharArray();
//            int index = 0;

//            int currentLat = 0;
//            int currentLng = 0;
//            int next5bits;
//            int sum;
//            int shifter;

//            try
//            {
//                while (index < polyChars.Length)
//                {
//                    // calculate next latitude
//                    sum = 0;
//                    shifter = 0;
//                    do
//                    {
//                        next5bits = (int)polyChars[index++] - 63;
//                        sum |= (next5bits & 31) << shifter;
//                        shifter += 5;
//                    } while (next5bits >= 32 && index < polyChars.Length);

//                    if (index >= polyChars.Length)
//                        break;

//                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

//                    //calculate next longitude
//                    sum = 0;
//                    shifter = 0;
//                    do
//                    {
//                        next5bits = (int)polyChars[index++] - 63;
//                        sum |= (next5bits & 31) << shifter;
//                        shifter += 5;
//                    } while (next5bits >= 32 && index < polyChars.Length);

//                    if (index >= polyChars.Length && next5bits >= 32)
//                        break;

//                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
//                    var p = new LatLng(Convert.ToDouble(currentLat) / 100000.0, 
//                                       Convert.ToDouble(currentLng) / 100000.0);
//                    poly.Add(p);
//                }
//            }
//            catch
//            {
//                RunOnUiThread(() =>
//                  Toast.MakeText(this, "Please wait...", ToastLength.Short).Show());
//            }
//            return poly;
//        }

//        async Task<string> DirectionHttpRequest(string url)
//        {
//            webclient = new WebClient();

//            string result;

//            try
//            {
//                result = await webclient.DownloadStringTaskAsync(new Uri(url));
//            }
//            catch (Exception e)
//            {
//                result = e.ToString();
//            }
//            finally
//            {
//                if (webclient != null)
//                {
//                    webclient.Dispose();
//                    webclient = null;
//                }
//            }

//            return result;
//        }
//    }
//}
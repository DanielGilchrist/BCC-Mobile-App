using System;
using System.Threading;
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
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using BCC_Bridge.Android.Maps;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;

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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
            vInput.EditorAction += VehicleInput_EditorAction;
            vehicleHeight = 4;
            placingBridgeMarkers = false;

            bridgeService = new BridgeService();
            bridges = bridgeService.All();
            badBridges = new List<Bridge>();

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
                    Toast.MakeText(this, "Please wait for bridge markers to be placed before entering a new value", ToastLength.Short).Show();
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
            InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
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
                Toast.MakeText(this, "Invalid Location", ToastLength.Short).Show();
            }
        }

        private IList<Address> GetCoordsFromName(string name)
        {
            var geo = new Geocoder(this);
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
            RunOnUiThread(() => gMap.Clear());

            RunOnUiThread(() => Toast.MakeText(this, "Loading Bridge Markers...", ToastLength.Short).Show());
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
                RunOnUiThread(() => SetMarker(type, bridges[i].Signed_Clearance.ToString(), bridges[i].Latitude, bridges[i].Longitude, false));
            }

            placingBridgeMarkers = false;
            RunOnUiThread(() => Toast.MakeText(this, "Bridge Markers Loaded", ToastLength.Short).Show());

            if (destination != null)
            {
                RunOnUiThread(() => ProcessRoute());
            }
        }

        public string MakeDirectionURL(double originLatitude, double originLongitude, double destLatitude, double destLongitude)
        {
            StringBuilder url = new StringBuilder();
            /*url.Append("https://maps.googleapis.com/maps/api/directions/json");
            url.Append("?origin=");// from
            url.Append(originLatitude);
            url.Append(",");
            url.Append(originLongitude);
            url.Append("&destination=");// to
            url.Append(destLatitude);
            url.Append(",");
            url.Append(destLongitude);
            url.Append("&mode=driving&alternatives=true");
            url.Append("&key=AIzaSyAtYVVEVhHpesj31u0VVRBjwUzC6Z25lms");*/

            url.Append("http://route.cit.api.here.com/routing/7.2/calculateroute.json");
            url.Append("?app_id=YueFlTt5s8iXeXb0VZPx");
            url.Append("&app_code=sG2iAqVYSywf0KjhpK1drA");
            url.AppendFormat("&waypoint0=geo!{0},{1}", originLatitude, originLongitude);
            url.AppendFormat("&waypoint1=geo!{0},{1}", destLatitude, destLongitude);
            url.Append("&mode=fastest;truck;traffic:disabled");
            //url.Append("&avoidareas=");

            Console.WriteLine(url.ToString());

            return url.ToString();
        }

        private void ProcessRoute()
        {
            var oAddress = GetCoordsFromName("Queensland University of Technology");
            //var oAddress = GetMyLocation();
            var dAddress = destination;

            double oLat = oAddress[0].Latitude, oLong = oAddress[0].Longitude;
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

            RunOnUiThread(() => {
                SetMarker(MarkerType.Normal, "Origin", origin.Latitude, origin.Longitude, false);
                SetMarker(MarkerType.Normal, "Destination", destination.Latitude, destination.Longitude, false);
            });

            SetDirectionQuery(DirectionJSONResponse);
        }

        private void SetDirectionQuery(string response)
        {
            var routesObject = JsonConvert.DeserializeObject<HereJSONResponse>(response).response;
           
            if (routesObject.route.Count > 0)
            {
                var routes = routesObject.route;

                bool isOkay = false;
               /* Console.WriteLine("Number of routes: " + routes.Count);

                int routeIndex = 0;
                bool isOkay = false;
                while (routeIndex < routes.Count)
                {
                    
                    var legs = routes[routeIndex].leg;
                    for (int i = 0; i < legs.Count; i++)
                    {
                        double startLat = legs[i].start_location.lat, startLong = legs[i].start_location.lng;
                        double endLat = legs[i].end_location.lat, endLong = legs[i].end_location.lng;

                        var boundBuilder = new LatLngBounds.Builder();
                        boundBuilder.Include(new LatLng(routes[routeIndex].bounds.northeast.lat, 
                                                        routes[routeIndex].bounds.northeast.lng));
                        boundBuilder.Include(new LatLng(routes[routeIndex].bounds.southwest.lat,
                                                        routes[routeIndex].bounds.southwest.lng));

                        //var bound = boundBuilder.Build();

                        foreach(var location in badBridges)
                        {
                            if (!(bound.Contains(new LatLng(location.Latitude, location.Longitude))))
                            {
                                isOkay = true;
                            }

                            // check if bridge is on leg or route
                            if(Math.Abs((endLat - startLat) * (location.Longitude - startLong) - (endLong - startLong) * (location.Latitude - startLat)) < 1)
                            {
                                isOkay = true;
                            }
                        }
                    }

                    if (isOkay == true)
                    {
                        Console.WriteLine("Broke from loop with routeIndex = " + routeIndex);
                        break;
                    }

                    routeIndex++;
                }*/

                /*if (routeIndex >= routes.Count)
                {
                    routeIndex--;
                }

                Console.WriteLine("Route Index: " + routeIndex); */

                if (isOkay == true)
                {
                    Toast.MakeText(this, "Unable to find acceptable route", ToastLength.Long).Show();
                }
                else
                {
                    //string encodedPoints = routes[0].overview_polyline.points;
                    //var decodedPoints = DecodePolyPoints(encodedPoints);

                    var points = new LatLng[routes[0].waypoint.Count];


                    for (int i = 0; i < routes[0].waypoint.Count; i++)
                    {
                        points[i] = new LatLng(routes[0].waypoint[i].mappedPosition.latitude, routes[0].waypoint[i].mappedPosition.latitude);
                    }

                    /*int index = 0;
                    foreach (LatLng location in decodedPoints)
                    {
                        points[index++] = new LatLng(location.Latitude, location.Longitude);
                    }*/

                    var polyOption = new PolylineOptions();
                    polyOption.InvokeColor(Color.Red);
                    polyOption.InvokeWidth(5);
                    polyOption.Geodesic(true);
                    polyOption.Visible(true);
                    polyOption.Add(points);

                    RunOnUiThread(() => gMap.AddPolyline(polyOption));
                }

                
            }
            else
            {
                Toast.MakeText(this, "Unable to find acceptable route", ToastLength.Long).Show();
            }
        }

        private List<LatLng> DecodePolyPoints(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints)) { return null; }

            var poly = new List<LatLng>();
            char[] polyChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polyChars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polyChars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polyChars.Length);

                    if (index >= polyChars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polyChars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polyChars.Length);

                    if (index >= polyChars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    var p = new LatLng(Convert.ToDouble(currentLat) / 100000.0, 
                                       Convert.ToDouble(currentLng) / 100000.0);
                    poly.Add(p);
                }
            }
            catch
            {
                RunOnUiThread(() =>
                  Toast.MakeText(this, "Please wait...", ToastLength.Short).Show());
            }
            return poly;
        }

        async Task<string> DirectionHttpRequest(string url)
        {
            webclient = new WebClient();

            string result;

            try
            {
                result = await webclient.DownloadStringTaskAsync(new Uri(url));
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Unable to draw route. Please try again.", ToastLength.Short);
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
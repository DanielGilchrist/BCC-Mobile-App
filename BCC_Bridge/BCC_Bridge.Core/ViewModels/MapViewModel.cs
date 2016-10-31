using MvvmCross.Core.ViewModels;
using BCC_Bridge.Core.Interfaces;
using BCC_Bridge.Core.Models;
using System;
using System.Threading.Tasks;

namespace BCC_Bridge.Core.ViewModels
{
    public class MapViewModel : MvxViewModel
    {
        private GeoLocation myLocation;
        private IGeoCoder geocoder;
        private Action<GeoLocation, float> setMyLocation;
        private Action<GeoLocation> setMyLocationMarker;

        public MapViewModel(IGeoCoder geocoder)
        {
            this.geocoder = geocoder;
        }

        public GeoLocation MyLocation
        {
            get { return myLocation; }
            set { myLocation = value; }
        }

        public void OnMyLocationChanged(GeoLocation location)
        {
            MyLocation = location;
            GetLocationInfo(location);
        }

        private async Task GetLocationInfo(GeoLocation location)
        {
            var city = await geocoder.GetCityFromLocation(location);
            location.Locality = city;
            setMyLocationMarker(location);
        }

        public void OnMapSetup(Action<GeoLocation, float> SetMyLocation, Action<GeoLocation> SetMyLocationMarker)
        {
            setMyLocation = SetMyLocation;
            setMyLocationMarker = SetMyLocationMarker;
        }
    }
}
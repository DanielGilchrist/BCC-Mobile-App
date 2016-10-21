using SQLite;

namespace BCC_Bridge.Core
{
    public class Bridge
    {
	    public string Description { get; set; }
		public string Direction { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double Signed_Clearance { get; set; }
		public string Street_Name { get; set; }
		public string Suburb { get; set; }
    }
}
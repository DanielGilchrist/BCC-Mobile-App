using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using BCC_Bridge.Core;


// HOW TO USE:

// private readonly IBridgeService _bridgeService;

// public whateverMethod(IBridgeService bridgeService)
// 		{
//			_bridgeService = bridgeService;
//
//			var bridges = _bridgeCollection.All();
//
//			foreach (Bridge bridge in bridges)
//			{
//				Debug.WriteLine(bridge.Suburb);
//			}
// 		}



namespace MvxSqlite.Services
{
	
	public class BridgeService : IBridgeService
    {
		private List<Bridge> _bridges;

        public BridgeService()
        {
			var assembly = typeof(Bridge).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("BCC_Bridge.Core.Services.BridgeData.json");

			using (StreamReader r = new StreamReader(stream))
			{
				string json = r.ReadToEnd();
				this._bridges = JsonConvert.DeserializeObject<List<Bridge>>(json);
			}
        }

		public List<Bridge> All()
		{
			return this._bridges;
        }
    }
}
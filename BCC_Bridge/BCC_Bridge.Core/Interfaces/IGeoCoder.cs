using BCC_Bridge.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCC_Bridge.Core.Interfaces
{
    public interface IGeoCoder
    {
        Task<string> GetCityFromLocation(GeoLocation location);
    }
}

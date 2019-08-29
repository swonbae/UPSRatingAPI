using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UPSRatingAPI.UPSRateRef;
using UPSRatingAPI.UPSTimeRef;

namespace UPSRatingAPI.Models
{
    public class RatePackage
    {
        public RateResponse rateResponse { get; set; }
        public TransitResponseType timeResponse { get; set; }
    }
}
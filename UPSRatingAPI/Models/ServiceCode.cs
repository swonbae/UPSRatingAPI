using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UPSRatingAPI.Models
{
    public static class ServiceCode
    {
        //public int number { get; set; }
        //public string name { get; set; }

        static Dictionary<string, string> codeName = new Dictionary<string, string>();

        static ServiceCode()
        {
            codeName.Add("02", "UPS Expedited");
            codeName.Add("13", "UPS Express Saver");
            codeName.Add("12", "UPS 3 Day Select");
            codeName.Add("70", "UPS Access Point Economy");
            codeName.Add("01", "UPS Express");
            codeName.Add("14", "UPS Express Early");
            codeName.Add("65", "UPS Express Saver");
            codeName.Add("11", "UPS Standard");
            codeName.Add("08", "UPS Worldwide Expedited");
            codeName.Add("07", "UPS Worldwide Express");
            codeName.Add("54", "UPS Worldwide Express Plus");
        }

        public static string GetName(string code)
        {
            string result = "";

            if (codeName.ContainsKey(code))
            {
                result = codeName[code];
            }

            return result;
        }
    }
}
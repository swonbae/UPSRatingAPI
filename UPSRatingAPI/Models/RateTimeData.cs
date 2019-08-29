using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UPSRatingAPI.Models
{
    public class RateTimeData
    {
        private string serviceName { get; set; }
        private string estimatedTime { get; set; }
        public int estimatedTime_num { get; set; }    // for ordering
        private string totalCharge { get; set; }

        public RateTimeData(string name, string time, int time_num, string charge)
        {
            serviceName = name;
            estimatedTime = time;
            estimatedTime_num = time_num;
            totalCharge = charge;
        }

        public string SerivceName()
        {
            return serviceName;
        }

        public string EstimatedTime()
        {
            return estimatedTime;
        }

        public string TotalCharge()
        {
            return totalCharge;
        }
    }
}
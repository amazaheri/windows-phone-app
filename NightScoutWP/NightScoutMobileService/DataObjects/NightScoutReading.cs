using Microsoft.WindowsAzure.Mobile.Service;
using System;

namespace NightScoutMobileService.DataObjects
{
    public class NightScoutReading : DocumentData
    {
        public int sgv { get; set; }
        public string direction { get; set; }
        public string type { get; set; }
        public string device { get; set; }
        public string dateString { get; set; }
        public int filtered { get; set; }
        public int unfiltered { get; set; }
        public int rssi { get; set; }
        public DateTime date { get; set; } 
    }
}
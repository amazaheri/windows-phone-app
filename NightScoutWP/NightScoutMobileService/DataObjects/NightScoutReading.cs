using Microsoft.WindowsAzure.Mobile.Service;
using System;
using Newtonsoft.Json;

namespace NightScoutMobileService.DataObjects
{
    public class NightScoutReading : DocumentData
    {
        [JsonProperty(PropertyName = "sgv")]
        public int sgv { get; set; }
        [JsonProperty(PropertyName = "direction")]
        public string direction { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }
        [JsonProperty(PropertyName = "device")]
        public string device { get; set; }
        [JsonProperty(PropertyName = "dateString")]
        public string dateString { get; set; }
        [JsonProperty(PropertyName = "filtered")]
        public int filtered { get; set; }
        [JsonProperty(PropertyName = "unfiltered")]
        public int unfiltered { get; set; }
        [JsonProperty(PropertyName = "rssi")]
        public int rssi { get; set; }
        [JsonProperty(PropertyName = "date")]
        public DateTime date { get; set; }
        [JsonProperty(PropertyName = "mbg")]
        public int mbg { get; set; }
        [JsonProperty(PropertyName = "slope")]
        public double slope { get; set; }
        [JsonProperty(PropertyName = "scale")]
        public double scale { get; set; }
        [JsonProperty(PropertyName = "intercept")]
        public double intercept { get; set; }
    }
}
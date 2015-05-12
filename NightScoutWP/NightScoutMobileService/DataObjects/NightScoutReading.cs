using Microsoft.WindowsAzure.Mobile.Service;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace NightScoutMobileService.DataObjects
{
    public class NightScoutReading : DocumentData
    {
        [BsonElement("sgv")]
        public int Sgv { get; set; }
        [BsonElement("direction")]
        public string Direction { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("device")]
        public string Device { get; set; }
        [BsonElement("dateString")]
        public string DateString { get; set; }
        [BsonElement("filtered")]
        public int Filtered { get; set; }
        [BsonElement("unfiltered")]
        public int Unfiltered { get; set; }
        [BsonElement("rssi")]
        public int Rssi { get; set; }
        [BsonElement("date")]
        public long Date { get; set; }
        [BsonElement("mbg")]
        public int Mbg { get; set; }
        [BsonElement("slope")]
        public double Slope { get; set; }
        [BsonElement("scale")]
        public double Scale { get; set; }
        [BsonElement("intercept")]
        public double Intercept { get; set; }
        [BsonElement("noise")]
        public double Noise { get; set; }

    }
}
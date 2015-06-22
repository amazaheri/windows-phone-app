using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.ScheduledJobs;
using NightScoutMobileService.Models;
using NightScoutMobileService.DataObjects;
using Microsoft.ServiceBus.Notifications;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Twilio;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NightScoutMobileService
{
    // A simple scheduled job which can be invoked manually by submitting an HTTP
    // POST request to the path "/jobs/sample".

    public class NightScoutJob : ScheduledJob
    {

        public async override Task ExecuteAsync()
        {
            long bg;
            long rawbg = 0;
            string direction = "-";
            string battery;
            int bgdelta;
            long filtered;
            long unfiltered;
            long slope;
            long intercept;
            int scale;
            DateTime date;
            TimeSpan lastReading;
            int rawCalcOffset = 5;
            long currentRatio = 0;
            bool sendAlert = false;

            string pebbleEndPoint = this.Services.Settings["Pebble"];
            int sgvHigh = int.Parse(this.Services.Settings["SgvHigh"]);
            int sgvLow = int.Parse(this.Services.Settings["SgvLow"]);
            bool pushNotification = bool.Parse(this.Services.Settings["PushNotification"]);
            bool SMSNotification = bool.Parse(this.Services.Settings["SMSNotification"]);
            bool raw = false;


            try
            {
                //Direct access to MongoDB
                //var client = new MongoClient(this.Services.Settings.Connections["Mongo"].ConnectionString);
                //var server = client.GetServer();
                //var database = server.GetDatabase(this.Services.Settings["MongoDB"]);
                //var collection = database.GetCollection<NightScoutReading>(this.Services.Settings["MongoCollection"]);
                //var query = collection.AsQueryable<NightScoutReading>().Where(e => e.Type == "sgv").Last<NightScoutReading>();
                //string direction = "-";

                HttpClient client = new HttpClient();
                string jsonResponse = await client.GetStringAsync(pebbleEndPoint);
                JObject response = JObject.Parse(jsonResponse);

                bg = long.Parse(response["bgs"][0]["sgv"].ToString());
                battery = response["bgs"][0]["battery"].ToString();
                direction = response["bgs"][0]["direction"].ToString();
                bgdelta = int.Parse(response["bgs"][0]["bgdelta"].ToString());
                filtered = long.Parse(response["bgs"][0]["filtered"].ToString());
                unfiltered = long.Parse(response["bgs"][0]["unfiltered"].ToString());
                date = new DateTime(1970, 01, 01).AddMilliseconds(long.Parse(response["bgs"][0]["datetime"].ToString()));
                lastReading = DateTime.Now - date;

                if (response["cals"][0]["slope"] != null)
                {
                    slope = long.Parse(response["cals"][0]["slope"].ToString());
                    intercept = long.Parse(response["cals"][0]["intercept"].ToString());
                    scale = int.Parse(response["cals"][0]["scale"].ToString());
                    if (bg < 10)
                    {
                        //currentRatio = (scale * (filtered - intercept) / slope / (bg * 1 + rawCalcOffset * 1));
                        //rawbg = ((scale * (unfiltered - intercept) / slope / currentRatio) * 1 - rawCalcOffset * 1);
                        //rawbg = ((scale * (unfiltered - intercept) / slope) * 1 - rawCalcOffset * 1);
                        rawbg = scale * (unfiltered - intercept) / slope;
                        raw = true;

                    }
                }


                switch (direction.ToLower())
                {
                    case "singleup":
                        direction = '\x25B2'.ToString();
                        break;
                    case "doubleup":
                        direction = string.Concat('\x25B2', '\x25B2');
                        sendAlert = true;
                        break;
                    case "fortyfiveup":
                        direction = '\x25B3'.ToString();
                        break;
                    case "singledown":
                        direction = '\x25BC'.ToString();
                        break;
                    case "doubledown":
                        direction = string.Concat('\x25BC', '\x25BC');
                        sendAlert = true;
                        break;
                    case "fortyfivedown":
                        direction = '\x25BD'.ToString();
                        break;

                }
                if ((bg <= sgvLow) || (bg >= sgvHigh) || sendAlert)
                {

                    string notification = String.Format("{0}: {1} {2} mg/dL {3} {4} min(s) ago", (raw ? "raw bg" : "bg"), (raw ? rawbg : bg), ((bgdelta > 0 ? '+' : ' ') + bgdelta.ToString()), direction, lastReading.Minutes);

                    if (pushNotification)
                    {
                        await SendPushNotification(notification);
                    }

                    if (SMSNotification)
                    {
                        await SendSms(notification);
                    }
                }

            }
            catch (Exception ex)
            {
                this.Services.Log.Error(ex.Message);
            }
        }

        private async Task SendSms(string notification)
        {
            string accountSID = this.Services.Settings["TwilioSID"];
            string authToken = this.Services.Settings["TwilioToken"];
            // Create an instance of the Twilio client.
            TwilioRestClient tClient;
            tClient = new TwilioRestClient(accountSID, authToken);

            // Send an SMS message.
            SMSMessage result = await tClient.SendSmsMessageAsync(
                this.Services.Settings["TwilioFrom"], this.Services.Settings["TwilioTo"], notification);

            if (result.RestException != null)
            {
                //an exception occurred making the REST call
                string message = result.RestException.Message;
            }
        }

        private async Task SendPushNotification(string notification)
        {
            string NHConnection = this.Services.Settings.Connections["NHConnectionString"].ConnectionString;
            string NHPath = this.Services.Settings["NHPath"].ToString();
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(NHConnection, NHPath);
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + notification + "</text></binding></visual></toast>";
            await hub.SendWindowsNativeNotificationAsync(toast);
        }
    }

}



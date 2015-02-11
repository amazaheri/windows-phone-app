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


namespace NightScoutMobileService
{
    // A simple scheduled job which can be invoked manually by submitting an HTTP
    // POST request to the path "/jobs/sample".

    public class NightScoutJob : ScheduledJob
    {
        private string lastReadingId = "";

        public async override Task ExecuteAsync()
        {
            
            
            try
            {
                var client = new MongoClient(this.Services.Settings.Connections["mongo"].ConnectionString);
                var server = client.GetServer();
                var database = server.GetDatabase("sithlordsam");
                var collection = database.GetCollection<NightScoutReading>("sithlordsam");
                var query = collection.AsQueryable<NightScoutReading>().Where(e => e.Type == "sgv").Last<NightScoutReading>();
                string direction = "-";              
                        

                if (lastReadingId != query.Id)
                {
                    if ((query.Sgv <= 70) || (query.Sgv >= 180))
                    {
                        switch (query.Direction.ToLower())
                        {
                            case "singleup":
                                direction = '\x25B2'.ToString();
                                break;
                            case "doubleup":
                                direction = string.Concat('\x25B2', '\x25B2');
                                break;
                            case "fortyfiveup":
                                direction = '\x25B3'.ToString();
                                break;
                            case "singledown":
                                direction = '\x25BC'.ToString();
                                break;
                            case "doubledown":
                                direction = string.Concat('\x25BC', '\x25BC');
                                break;
                            case "fortyfivedown":
                                direction = '\x25BD'.ToString();
                                break;
                            case "NOT COMPUTABLE": break;
                        }

                        lastReadingId = query.Id;

                        string notification = String.Format("Current bg: {0} Trend: {1}", query.Sgv, direction);
                        NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://nightscoutmobilehub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=xl1zRhF0EKmoZdkHdvzGT+RCx7YpzGopDnRjrDpp6QA=", "nightscoutmobilehub");
                        var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + notification + "</text></binding></visual></toast>";
                        await hub.SendWindowsNativeNotificationAsync(toast);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Services.Log.Error(ex.Message);

            }

        }

    }

}



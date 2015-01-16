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


        public async override Task ExecuteAsync()
        {

            var client = new MongoClient(this.Services.Settings.Connections["mongo"].ConnectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("sithlordsam");
            var collection = database.GetCollection<NightScoutReading>("sithlordsam");
            var query = collection.AsQueryable<NightScoutReading>().Where(e => e.Type == "sgv").OrderBy(e => e.Date).First<NightScoutReading>();
            char direction = '-';
           
            if ((query.Sgv <= 70) || (query.Sgv >= 180))
            {
                if (query.Direction.ToLower().Contains("up"))
                    direction = '\x25B2';
                else
                        if (query.Direction.ToLower().Contains("down"))
                    direction = '\x25BC';
                string notification = String.Format("Current bg: {0} Trend: {1}", query.Sgv, direction.ToString());
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://nightscoutmobilehub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=xl1zRhF0EKmoZdkHdvzGT+RCx7YpzGopDnRjrDpp6QA=", "nightscoutmobilehub");
                var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + notification + "</text></binding></visual></toast>";
                await hub.SendWindowsNativeNotificationAsync(toast);
            }

        }

    }

}

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;


namespace Test.Harness
{
    class Program
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
          "https://nightscoutmobile.azure-mobile.net/", "nYbTwughndskVWTjEGMaemXABjYWlL11");

        
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            char direction = '-';
            try
            {
                IMobileServiceTable<NightScoutReading> table = MobileService.GetTable<NightScoutReading>();
                //NightScoutReading reading = await table.LookupAsync("5484f78529b19f32387ffa07");
                //Console.WriteLine(reading.sgv);
                //Console.Read();
                List<NightScoutReading> items = await table.Where(O=>O.type== "sgv").OrderByDescending(O=>O.date).ToListAsync();

                if (items[0].direction.ToLower().Contains("up"))
                    direction = '\x25B2';
                else
                    if (items[0].direction.ToLower().Contains("down"))
                    direction = '\x25BC';
                string notification = String.Format("Current bg: {0} Trend: {1}", items[0].sgv, direction.ToString());
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://nightscoutmobilehub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=xl1zRhF0EKmoZdkHdvzGT+RCx7YpzGopDnRjrDpp6QA=", "nightscoutmobilehub");
                var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">"+notification+"</text></binding></visual></toast>";
                await hub.SendWindowsNativeNotificationAsync(toast);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Console.WriteLine(ex.Request);
                Console.WriteLine(ex.Response);
            }
            Console.Read();
        }

    }
}

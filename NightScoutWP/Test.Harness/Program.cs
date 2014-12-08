using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NightScoutMobileService.DataObjects;

namespace Test.Harness
{
    class Program
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
          "https://nightscoutmobile.azure-mobile.net/", "PAwTZVMkmGBUuozXrQsFZAYfTNnYqB15");

        
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            try
            {
                IMobileServiceTable<NightScoutReading> table = MobileService.GetTable<NightScoutReading>();
                NightScoutReading reading = await table.LookupAsync("5484f78529b19f32387ffa07");
                Console.WriteLine(reading.sgv);
                Console.Read();
                //IMobileServiceTableQuery<NightScoutReading> query = table.Where(item => item.type != null && item.type == "sgv");
                //var items = await query.ToCollectionAsync();            

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

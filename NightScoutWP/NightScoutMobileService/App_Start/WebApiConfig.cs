using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using NightScoutMobileService.DataObjects;
using NightScoutMobileService.Models;

namespace NightScoutMobileService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new NightScoutMobileInitializer());
        }
    }

    public class NightScoutMobileInitializer : ClearDatabaseSchemaIfModelChanges<NightScoutMobileContext>
    {
        protected override void Seed(NightScoutMobileContext context)
        {
            List<NightScoutReading> items = new List<NightScoutReading>
            {
                new NightScoutReading { Id = Guid.NewGuid().ToString(), Sgv = 200, Direction = "Up", Type="sgv" , Date=1431447293000 , DateString="", Device="", Filtered=0, Intercept=0.0, Mbg=0,  Rssi=0 , Scale=0.0 , Slope=0.0 , Unfiltered= 0, Noise=0 },
            };

            foreach (NightScoutReading item in items)
            {
                context.Set<NightScoutReading>().Add(item);
            }

            base.Seed(context);
        }
    }
}


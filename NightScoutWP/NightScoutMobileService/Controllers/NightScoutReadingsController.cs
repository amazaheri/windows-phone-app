using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using NightScoutMobileService.DataObjects;
using NightScoutMobileService.Models;

namespace NightScoutMobileService.Controllers
{
    public class NightScoutReadingsController : TableController<NightScoutReading>
    {
        private bool connectionStringInitialized = false;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            string connectionStringName = "Mongo";
            string databaseName = "sithlordsam";
            string collectionName = "sithlordsam";
            InitializeConnectionString(connectionStringName);
            DomainManager = new MongoDomainManager<NightScoutReading>(connectionStringName, databaseName, collectionName, Request, Services);

        }

        private void InitializeConnectionString(string connectionStringName)
        {

            if (!connectionStringInitialized)
            {
                connectionStringInitialized = true;
                if (!this.Services.Settings.Connections.ContainsKey(connectionStringName))
                {
                    var connectionString = this.Services.Settings[connectionStringName];
                    var connectionSetting = new ConnectionSettings(connectionStringName, connectionString);
                    this.Services.Settings.Connections.Add(connectionStringName, connectionSetting);
                }
            }
        }

        // GET tables/NightScoutReadings
        public IQueryable<NightScoutReading> GetAllNightScoutReading()
        {
            return Query(); 
        }

        // GET tables/NightScoutReadings/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<NightScoutReading> GetNightScoutReading(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/NightScoutReadings/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<NightScoutReading> PatchNightScoutReading(string id, Delta<NightScoutReading> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/NightScoutReadings
        public async Task<IHttpActionResult> PostNightScoutReading(NightScoutReading item)
        {
            NightScoutReading current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/NightScoutReadings/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteNightScoutReading(string id)
        {
             return DeleteAsync(id);
        }

    }
}
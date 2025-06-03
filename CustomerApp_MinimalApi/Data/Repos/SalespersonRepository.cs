using CustomerApp_MinimalApi.Data.Entities;
using CustomerApp_MinimalApi.Data.Interfaces;
using Microsoft.Azure.Cosmos;

namespace CustomerApp_MinimalApi.Data.Repos
{
    public class SalespersonRepository : ISalespersonRepository
    {
        private readonly Container _container; //Cosmos DB container for Salesperson

        //Constructor receives app configuration - DI and connects to Cosmos DB container.
        public SalespersonRepository(IConfiguration config)
        {
            var client = new CosmosClient(config["CosmosDb:ConnectionString"]);
            var database = client.GetDatabase(config["CosmosDb:DatabaseName"]);
            _container = database.GetContainer(config["CosmosDb:SalespersonContainer"]);
        }


        //Retrieves all salespeople from the container
        public async Task<IEnumerable<Salesperson>> GetAllSalespersonsAsync()
        {
            var query = _container.GetItemQueryIterator<Salesperson>();

            //Initializes list to collect results
            List<Salesperson> results = new();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }


        //Retrieves a single salesperson by ID
        public async Task<Salesperson?> GetSalespersonByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Salesperson>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}

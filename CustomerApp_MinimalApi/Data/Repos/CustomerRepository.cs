using CustomerApp_MinimalApi.Data.Entities;
using CustomerApp_MinimalApi.Data.Interfaces;
using Microsoft.Azure.Cosmos;
using Shared_Model;

namespace CustomerApp_MinimalApi.Data.Repos
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Container _container; //For cosmosDB container for Customers

        //Initializes CosmosDB container using app configuration settings.
        public CustomerRepository(IConfiguration config)
        {
            var client = new CosmosClient(config["CosmosDb:ConnectionString"]);
            var database = client.GetDatabase(config["CosmosDb:DatabaseName"]);
            _container = database.GetContainer(config["CosmosDb:CustomerContainer"]);
        }

        //Add a new customer to the container
        public async Task AddCustomerAsync(Customer customer)
        {
            await _container.CreateItemAsync(customer, new PartitionKey(customer.Id));
        }

        //Retrieve all customers from the container.
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>();
            List<Customer> results = new();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }


        //Get a single customer by ID.
        public async Task<Customer?> GetCustomerByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Customer>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }


        // Update an existing customer - update or insert
        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _container.UpsertItemAsync(customer, new PartitionKey(customer.Id));
        }


        //Delete a customer by ID.
        public async Task DeleteCustomerAsync(string id)
        {
            await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
        }


        //Search for customers by name and/or salespersonId.
        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string? name, string? salespersonId)
        {
            var queryString = "SELECT * FROM c WHERE 1=1"; //Start with true condition

            if (!string.IsNullOrWhiteSpace(name))
                queryString += $" AND CONTAINS(c.Name, '{name}', true)"; //Filter by name case-insensitive

            if (!string.IsNullOrWhiteSpace(salespersonId))
                queryString += $" AND c.SalespersonId = '{salespersonId}'"; //Filter by salesperson ID.

            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(queryString));
            List<Customer> results = new();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}

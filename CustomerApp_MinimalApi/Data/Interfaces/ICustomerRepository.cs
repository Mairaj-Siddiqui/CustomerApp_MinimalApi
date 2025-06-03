using CustomerApp_MinimalApi.Data.Entities;
using Shared_Model;

namespace CustomerApp_MinimalApi.Data.Interfaces
{
    public interface ICustomerRepository
    {

        //Interfaces defining a contract for any class that will handle customer data operations.
        Task AddCustomerAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(string id);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string id);


        // Search customer by name and/or salesperson ID.
        Task<IEnumerable<Customer>> SearchCustomersAsync(string? name, string? salespersonId);
    }
}

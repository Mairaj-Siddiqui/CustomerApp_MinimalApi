using CustomerApp_MinimalApi.Data.Entities;

namespace CustomerApp_MinimalApi.Data.Interfaces
{
    public interface ISalespersonRepository
    {

        // single salesperson by their unique ID.
        Task<Salesperson?> GetSalespersonByIdAsync(string id);

        //all salespersons from database
        Task<IEnumerable<Salesperson>> GetAllSalespersonsAsync();
    }
}

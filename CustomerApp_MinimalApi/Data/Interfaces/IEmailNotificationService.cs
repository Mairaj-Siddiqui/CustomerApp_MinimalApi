using CustomerApp_MinimalApi.Data.Entities;
using Shared_Model;
using System.Threading.Tasks;

namespace CustomerApp_MinimalApi.Data.Interfaces
{
    public interface IEmailNotificationService
    {
        //Imports the Customer and Salesperson entity classes
        //email notification to the salesperson when a customer is created or updated

        //Customer: whose info will be included in email.
        //Salesperson: who will receive notification email.
        Task NotifySalespersonAsync(Customer customer, Salesperson salesperson);
    }
}

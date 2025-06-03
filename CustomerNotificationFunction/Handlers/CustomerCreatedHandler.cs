using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shared_Model; // using shared model for Customer

namespace CustomerNotificationFunction;

public class CustomerCreatedHandler
{
    private readonly ILogger<CustomerCreatedHandler> _logger;
    private readonly IConfiguration _config;

    public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [Function("CustomerCreatedHandler")]
    public async Task Run(
        [CosmosDBTrigger(
            databaseName: "CustomerDB",
            containerName: "Customers",
            Connection = "CosmosDbConnectionString",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Customer> input,
        FunctionContext context)
    {
        if (input is null || input.Count == 0)
        {
            _logger.LogInformation("No documents received.");
            return;
        }

        //------------------------------------------------

        //LOGGING FOR DEBUGGING
        _logger.LogInformation("Cosmos DB Trigger received event.");
        foreach (var customer in input)
        {
            _logger.LogInformation($"Customer: {customer.Name}, ID: {customer.Id}");
        }
        if (input is null || input.Count == 0)
        {
            _logger.LogInformation("No documents to process.");
            return;
        }

        //------------------------------------------------

        var apiKey = _config["SendGridApiKey"];
        var fromEmail = _config["SendGridFromEmail"];
        var fallback = _config["SalespersonEmailFallback"];

        var client = new SendGridClient(apiKey);

        foreach (var customer in input)
        {
            var from = new EmailAddress(fromEmail, "Customer Notification");
            var to = new EmailAddress(fallback, "Salesperson"); // Always to fallback for now

            var subject = $"AzureF - New/Updated Customer: {customer.Name}";
            var plain = $"Name: {customer.Name}\nEmail: {customer.Email}";
            var html = $"<strong>Customer:</strong> {customer.Name}<br>Email: {customer.Email}";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plain, html);
            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation($"Email sent for {customer.Name}, Status: {response.StatusCode}");
        }
    }
}

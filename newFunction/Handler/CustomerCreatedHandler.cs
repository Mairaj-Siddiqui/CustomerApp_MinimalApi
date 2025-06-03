using System;
using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail; 
using Shared_Model;

namespace CustomerNotificationFunction.Handler;

public class CustomerCreatedHandler
{

    private readonly IConfiguration _config;
    private readonly ILogger<CustomerCreatedHandler> _logger;

    public CustomerCreatedHandler(IConfiguration config, ILogger<CustomerCreatedHandler> logger)
    {
        _config = config;
        _logger = logger;
    }


    //function entry point - Executes automatically when data changes
    [Function("CustomerCreatedHandler")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "CustomerDB",
        containerName: "Customers",
        Connection = "cosmosconn",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Customer> input)
    {
        if (input != null && input.Count > 0)
        {
            _logger.LogInformation("First document Id: " + input[0].Name);
        }

        //Reads SendGrid config values from environment variables
        string apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        string fromEmail = Environment.GetEnvironmentVariable("SendGridFromEmail");
        string fallback = Environment.GetEnvironmentVariable("SalespersonEmailFallback");

        var client = new SendGridClient(apiKey);

        foreach (var customer in input)
        {
            var from = new EmailAddress(fromEmail, "Customer Notification");
            var to = new EmailAddress(fallback, "Salesperson"); // Always to fallback for now


            
            var subject = $"AzureF - New/Updated Customer: {customer.Name}";
            var plain = $"Name: {customer.Name}\nEmail: {customer.Email}";
            var html = $"<strong>Customer:</strong> {customer.Name}<br>Email: {customer.Email}";

            //Email notifyer - appearance
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plain, html);
            var response = await client.SendEmailAsync(msg);


            //Logs the SendGrid response status
            _logger.LogInformation($"Email sent for {customer.Name}, Status: {response.StatusCode}");
        }

    }
}
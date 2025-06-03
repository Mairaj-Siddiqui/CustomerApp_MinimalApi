using CustomerApp_MinimalApi.Data.Entities;
using CustomerApp_MinimalApi.Data.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shared_Model;

namespace CustomerApp_MinimalApi.Data.Repos
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IConfiguration config, ILogger<EmailNotificationService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task NotifySalespersonAsync(Customer customer, Salesperson salesperson)
        {

            //Reads SendGrid credentials and fallback email from appsettings.json
            var apiKey = _config["SendGridApiKey"];
            var fromEmail = _config["SendGridFromEmail"];
            var fallback = _config["SalespersonEmailFallback"];

            //Hardcoded real email for debug/testing fallback email from appsettings.json
            var toEmail = !string.IsNullOrWhiteSpace(salesperson.Email)
                ? salesperson.Email
                : fallback;
                toEmail = "safarstory.contact@gmail.com";

            //var toEmail = !string.IsNullOrWhiteSpace(salesperson.Email) ? salesperson.Email : fallback;

            //Logging for diagnostics
            _logger.LogInformation("Preparing to send email...");
            _logger.LogInformation($"To: {toEmail}");
            _logger.LogInformation($"Salesperson Name: {salesperson.Name}");
            _logger.LogInformation($"Customer Name: {customer.Name}");


            //Initializes the SendGrid client with your API key
            var client = new SendGridClient(apiKey);

            //Prepares from and to addresses using the SendGrid.Helpers.Mail.EmailAddress
            var from = new EmailAddress(fromEmail, "Customer App");
            var to = new EmailAddress(toEmail, salesperson.Name);

            //Email Notification UI : 
            var subject = $"New/Updated Customer: {customer.Name}";
            var plainText = $"Name: {customer.Name}\nEmail: {customer.Email}\nPhone: {customer.Telephone}\nAddress: {customer.Address}";
            var html = $"<strong>Customer Info:</strong><br>Name: {customer.Name}<br>Email: {customer.Email}<br>Phone: {customer.Telephone}<br>Address: {customer.Address}";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, html);
            var response = await client.SendEmailAsync(msg);


            //Logs the status code from SendGrid to verify delivery success (Accepted or error)
            _logger.LogInformation($"Email sent! Status code: {response.StatusCode}");
        }
    }
}

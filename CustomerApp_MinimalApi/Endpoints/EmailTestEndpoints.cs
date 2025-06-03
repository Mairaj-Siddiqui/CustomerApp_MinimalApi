using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CustomerApp_MinimalApi.Endpoints
{
    public static class EmailTestEndpoints
    {
        public static void MapEmailTestEndpoint(this WebApplication app)
        {

            // Map a POST endpoint at /test/send-email
            //IConfiguration built-in .NET Core interface that
            //provides access to configuration settings as appsettings.json
            app.MapPost("/test/send-email", async ([FromServices] IConfiguration config) =>
            {

                //Read SendGrid configuration values from appsettings.json
                var apiKey = config["SendGridApiKey"];
                var fromEmail = config["SendGridFromEmail"];
                var toEmail = config["SalespersonEmailFallback"];


                //Validate that none of the required configuration values are missing
                if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(toEmail))
                    return Results.BadRequest("Missing email configuration.");


                //Initialize SendGrid client with API key
                var client = new SendGridClient(apiKey);

                //Setup sender and recipient email addresses
                var from = new EmailAddress(fromEmail, "Customer App");
                var to = new EmailAddress(toEmail, "Test Recipient");

                var subject = "SendGrid Email Test";
                var plainTextContent = "This is a test email from CustomerApp_MinimalApi using SendGrid.";
                var htmlContent = "<strong>This is a test email from CustomerApp_MinimalApi using SendGrid.</strong>";


                //build email format and send the email using SendGrid helper - MailHelper
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);


                //Return status message to the client
                return Results.Ok($"Email sent! Status code: {response.StatusCode}");
            });
        }
    }
}

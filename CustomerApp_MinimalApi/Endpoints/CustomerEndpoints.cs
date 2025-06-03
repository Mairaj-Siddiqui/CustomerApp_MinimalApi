using Azure.Core;
using System.Reflection;
using CustomerApp_MinimalApi.Data.Entities;
using CustomerApp_MinimalApi.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Shared_Model;

namespace CustomerApp_MinimalApi.Endpoints
{
    public static class CustomerEndpoints
    {

        //Extension Method for Mapping Endpoints call from Program.cs
        public static void MapCustomerEndpoints(this WebApplication app)
        {
            // POST: Add new customer
            app.MapPost("/customers", async (

                //Gets the customer data from request body and binds it to the DTO
                [FromBody] CustomerCreateDto input,

                //Injects services from DI container for data access and email notification.
                [FromServices] ICustomerRepository customerRepo,
                [FromServices] ISalespersonRepository salespersonRepo,
                [FromServices] IEmailNotificationService emailService) =>
            {
                //Maps the DTO input to a full Customer entity with a new GUID
                var customer = new Customer
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = input.Name,
                    Title = input.Title,
                    Telephone = input.Telephone,
                    Email = input.Email,
                    Address = input.Address,
                    SalespersonId = input.SalespersonId
                };

                var salesperson = await salespersonRepo.GetSalespersonByIdAsync(customer.SalespersonId);
                if (salesperson == null)
                    return Results.BadRequest("Salesperson not found.");

                // Saves the customer
                await customerRepo.AddCustomerAsync(customer);

                //------------------------------------------------------------------
                //notifies the salesperson via SendGrid.
                //await emailService.NotifySalespersonAsync(customer, salesperson);
                //------------------------------------------------------------------


                return Results.Created($"/customers/{customer.Id}", customer);

            });


            // PUT: Update customer
            app.MapPut("/customers/{id}", async (

            // Gets the customer data from request body and binds it to the DTO
            string id,                
                [FromBody] CustomerCreateDto input,


                [FromServices] ICustomerRepository customerRepo,
                [FromServices] ISalespersonRepository salespersonRepo,
                [FromServices] IEmailNotificationService emailService) =>
            {
                var existing = await customerRepo.GetCustomerByIdAsync(id);
                if (existing is null) return Results.NotFound();

                //Updates all fields of the entity
                existing.Name = input.Name;
                existing.Title = input.Title;
                existing.Telephone = input.Telephone;
                existing.Email = input.Email;
                existing.Address = input.Address;
                existing.SalespersonId = input.SalespersonId;

                //Persists the changes in Cosmos DB.
                await customerRepo.UpdateCustomerAsync(existing);


                //Sends email notification if the salesperson exists
                var salesperson = await salespersonRepo.GetSalespersonByIdAsync(existing.SalespersonId);


                //------------------------------------------------------------------
                //if (salesperson is not null)

                //notifies the salesperson via SendGrid.
                // await emailService.NotifySalespersonAsync(existing, salesperson);
                //------------------------------------------------------------------


                return Results.Ok(existing);
            });

            // DELETE: Remove customer
            app.MapDelete("/customers/{id}", async (
                string id,
                [FromServices] ICustomerRepository customerRepo) =>
            {
                var customer = await customerRepo.GetCustomerByIdAsync(id);
                if (customer is null) return Results.NotFound();

                await customerRepo.DeleteCustomerAsync(id);
                return Results.NoContent();
            });

            // GET: All customers
            app.MapGet("/customers", async (
                [FromServices] ICustomerRepository customerRepo) =>
            {
                var customers = await customerRepo.GetAllCustomersAsync();
                return Results.Ok(customers);
            });

            // GET: By Id
            app.MapGet("/customers/{id}", async (
                string id,
                [FromServices] ICustomerRepository customerRepo) =>
            {
                var customer = await customerRepo.GetCustomerByIdAsync(id);
                return customer is not null ? Results.Ok(customer) : Results.NotFound();
            });

            // GET: Search
            app.MapGet("/customers/search", async (

                //Filters customers by name and / or salesperson ID.
                [FromQuery] string? name,
                [FromQuery] string? salespersonId,
                [FromServices] ICustomerRepository customerRepo) =>
            {
                var result = await customerRepo.SearchCustomersAsync(name, salespersonId);
                return Results.Ok(result);
            });
        }
    }
}

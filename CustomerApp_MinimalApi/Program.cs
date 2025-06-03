using CustomerApp_MinimalApi.Data.Interfaces;
using CustomerApp_MinimalApi.Data.Repos;
using CustomerApp_MinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Registers the application configuration from appsettings.json
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Registers the interfaces and their implementations in the dependency injection container
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISalespersonRepository, SalespersonRepository>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


// Swagger
builder.Services.AddSwaggerGen(); //Generates the OpenAPI spec
builder.Services.AddEndpointsApiExplorer(); //Allows discovering minimal API endpoints for Swagger

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API V1");
    options.RoutePrefix = string.Empty; // <--- This is important to show Swagger at root
});

// ------Map Endpoints-------//

//--- // From CustomerEndpoints- POST, GET, PUT, DELETE routes - Entension method
app.MapCustomerEndpoints();


//--- // From SalespersonEndpoints -GET for all salespersons - Entension method
app.MapSalespersonEndpoints();


//POST endpoint to test SendGrid email directly
app.MapEmailTestEndpoint();  


app.Run();

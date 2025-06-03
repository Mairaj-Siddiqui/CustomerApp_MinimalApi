using CustomerApp_MinimalApi.Data.Entities;
using CustomerApp_MinimalApi.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

public static class SalespersonEndpoints
{

    //extension method to plug-in endpoint mappings into the app pipeline
    public static void MapSalespersonEndpoints(this WebApplication app)
    {

        //[FromServices] ISalespersonRepository - repo
        //Get services(like database or email helpers) via DI.
        app.MapGet("/salespersons", async ([FromServices] ISalespersonRepository repo) =>
        {
            var salespersons = await repo.GetAllSalespersonsAsync();
            return Results.Ok(salespersons);
        });
    }
}

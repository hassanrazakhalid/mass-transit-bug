using mass_transit_bug;
using mass_transit_bug.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddDbContext<AppDbContext>((sp, config) =>
{
    config.UseSqlServer(
        "Server=localhost;Database=mass-bug-database;User Id=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultiSubnetFailover=True");
});

services.AddOptions<SqlTransportOptions>().Configure(options =>
{
    options.Host = "localhost";
    options.Database = "mass-bug-database";
    options.Schema = "transport"; // the schema for the transport-related tables, etc. 
    options.Role = "transport";   // the role to assign for all created tables, functions, etc.
    options.Username = "masstransit";  // the application-level credentials to use
    options.Password = "H4rd2Gu3ss!";
    options.AdminUsername = "SA"; // the admin credentials to create the tables, etc.
    options.AdminPassword = "YourStrong@Passw0rd";
});
services.AddSqlServerMigrationHostedService(create: true, delete: false);
services.AddMassTransit(massTransit =>
{
    massTransit.AddConsumers(typeof(Program).Assembly);
            
    massTransit.AddEntityFrameworkOutbox<AppDbContext>(config =>
    {
        config.UseSqlServer();
        // config.QueryDelay = TimeSpan.FromSeconds(1);
       // Comment below line, then published events are received, but that defeats the whole purpose
        config.UseBusOutbox(x =>
        {
            //It should work with this
            //x.DisableDeliveryService();
        });
    });
    massTransit.UsingDb((context, cfg) =>
    {
        cfg.UseSqlServer(context);
        cfg.UseDbMessageScheduler();
        cfg.AutoStart = true;
        cfg.ConfigureEndpoints(context);
    });
            
    massTransit.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<AppDbContext>(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
app.MapGet("/", () => "Hello World!");

app.MapGet("/send-event", async (
    IPublishEndpoint publish
) =>
{
    await publish.Publish(new OrderCreated(Guid.NewGuid(), "Shoes", "Shoe-1"));
    return Results.Ok();
});

app.Run();

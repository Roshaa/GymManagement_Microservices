using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DotNetEnv;
using GymManagement_Members_Microservice.Client;
using GymManagement_Members_Microservice.Context;
using GymManagement_Members_Microservice.Data;
using GymManagement_Members_Microservice.Jwt;
using GymManagement_Members_Microservice.Mappers;
using GymManagement_Members_Microservice.Services.Background;
using GymManagement_Shared_Classes.Jwt;
using GymManagement_Shared_Classes.Swagger;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddGymSwagger("Members API");
builder.Services.AddGymJwtAuth(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardJwtHandler>();

builder.Services.AddHttpClient<PromoClient>(client =>
{
    var baseUrl = builder.Configuration["Apisettings:PromoService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Missing 'Apisettings:PromoService:BaseUrl'.");

    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<ForwardJwtHandler>();

builder.Services.AddHttpClient<MemberShipClient>(client =>
{
    var baseUrl = builder.Configuration["Apisettings:MemberShipService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Missing 'Apisettings:MemberShipService:BaseUrl'.");

    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
})
    .AddHttpMessageHandler<ForwardJwtHandler>();

string fqns = cfg["ServiceBus_Namespace"] ?? throw new("ServiceBus_Namespace missing");
string queue = cfg["ServiceBus_QueueName"] ?? throw new("ServiceBus_QueueName missing");

builder.Services.AddSingleton(new ServiceBusClient(
    fqns,
    new DefaultAzureCredential(),
    new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets }));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<ServiceBusClient>().CreateProcessor(queue, new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = 1,
        PrefetchCount = 0,
        MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
    }));

builder.Services.AddHostedService<SubscriptionPaymentsConsumerService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations.");
        throw;
    }

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var memberSeeder = new MemberSeeder();
        await memberSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding database");
        throw;
    }

}

if (app.Environment.IsDevelopment())
{
    app.UseGymSwaggerUI(title: "Gym Microservices API", version: "v1", serveAtRoot: true);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
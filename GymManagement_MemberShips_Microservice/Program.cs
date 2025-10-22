using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DotNetEnv;
using GymManagement_MemberShips_Microservice.Client;
using GymManagement_MemberShips_Microservice.Context;
using GymManagement_MemberShips_Microservice.Jwt;
using GymManagement_MemberShips_Microservice.Mappers;
using GymManagement_MemberShips_Microservice.Services.Background;
using GymManagement_Shared_Classes.Jwt;
using GymManagement_Shared_Classes.Migrations.NonSeeded;
using GymManagement_Shared_Classes.Swagger;
using Microsoft.EntityFrameworkCore;

//I wont seed this application's database for now
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

builder.Services.AddHttpClient<MemberDiscountClient>(client =>
{
    var baseUrl = builder.Configuration["Apisettings:MemberDiscountClient:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl)) throw new InvalidOperationException("Missing 'Apisettings:MemberDiscountClient:BaseUrl'.");

    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<ForwardJwtHandler>();

string fqns = cfg["ServiceBus_Namespace"] ?? throw new("ServiceBus_Namespace missing");
string queue = cfg["ServiceBus_QueueName"] ?? throw new("ServiceBus_QueueName missing");

builder.Services.AddSingleton(new ServiceBusClient(
    fqns,
    new DefaultAzureCredential(),
    new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets }));

builder.Services.AddSingleton(sp => sp.GetRequiredService<ServiceBusClient>().CreateSender(queue));

builder.Services.AddHostedService<SubscriptionPaymentsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsAsync<ApplicationDbContext>();
    app.UseGymSwaggerUI(title: "Gym Microservices API", version: "v1", serveAtRoot: true);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
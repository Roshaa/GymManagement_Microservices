using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DotNetEnv;
using GymManagement_MemberShips_Microservice.Client;
using GymManagement_MemberShips_Microservice.Context;
using GymManagement_MemberShips_Microservice.Jwt;
using GymManagement_MemberShips_Microservice.Mappers;
using GymManagement_MemberShips_Microservice.Services.Background;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

//I wont seed this application's database for now
Env.Load();

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration; // env vars are included by default

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gym Microservices API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT in the Authorization header. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });

});

var jwtSection = builder.Configuration.GetSection("Apisettings:JwtOptions");
builder.Services.Configure<JwtOptions>(jwtSection);
var jwt = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("JwtOptions missing");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

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
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gym Microservices API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
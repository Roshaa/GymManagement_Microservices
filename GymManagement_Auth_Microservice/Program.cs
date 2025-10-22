using GymManagement_Auth_Microservice.Context;
using GymManagement_Auth_Microservice.Jwt;
using GymManagement_Auth_Microservice.Mappers;
using GymManagement_Auth_Microservice.Services;
using GymManagement_Shared_Classes.Jwt;
using GymManagement_Shared_Classes.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityCore<IdentityUser>(opts =>
    {
        opts.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Apisettings:JwtOptions"));

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<JwtTokenGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGymSwagger("Members API");
builder.Services.AddGymJwtAuth(builder.Configuration);


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        IdentitySeeder identitySeeder = new IdentitySeeder();
        await identitySeeder.SeedAsync(services);
        logger.LogInformation("Identity roles/users seeded.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during migration/seeding.");
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

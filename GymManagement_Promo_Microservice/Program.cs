using GymManagement_Promo_Microservice.Context;
using GymManagement_Promo_Microservice.Mappers;
using GymManagement_Shared_Classes.Jwt;
using GymManagement_Shared_Classes.Migrations.NonSeeded;
using GymManagement_Shared_Classes.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGymSwagger("Members API");
builder.Services.AddGymJwtAuth(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(MappingConfig));

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
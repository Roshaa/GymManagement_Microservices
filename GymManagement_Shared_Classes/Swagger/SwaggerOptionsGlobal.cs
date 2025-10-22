using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace GymManagement_Shared_Classes.Swagger
{
    public static class SwaggerOptionsGlobal
    {
        public static IServiceCollection AddGymSwagger(this IServiceCollection services, string title, string version = "v1")
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT via Authorization header. Example: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
            });
            });
            return services;
        }
        public static WebApplication UseGymSwaggerUI(this WebApplication app, string title, string version = "v1", bool serveAtRoot = true)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{title} {version}");
                if (serveAtRoot) options.RoutePrefix = string.Empty;
                options.DisplayRequestDuration();
                options.DefaultModelsExpandDepth(-1);
            });
            return app;
        }
    }
}

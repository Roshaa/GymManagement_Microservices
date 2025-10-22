using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace GymManagement_Shared_Classes.Jwt
{
    public static class JwtAuth
    {

        public static IServiceCollection AddGymJwtAuth(this IServiceCollection services, IConfiguration config, string sectionPath = "Apisettings:JwtOptions",
        Action<JwtBearerOptions>? configure = null)
        {
            var jwt = config.GetSection(sectionPath).Get<JwtOptions>() ?? throw new InvalidOperationException($"Missing {sectionPath}");

            services.Configure<JwtOptions>(config.GetSection(sectionPath));

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = true;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
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
                configure?.Invoke(o);
            });

            services.AddAuthorization();
            return services;
        }
    }
}

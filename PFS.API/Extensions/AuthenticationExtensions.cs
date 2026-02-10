using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace PFS.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = jwtSettings["Key"]
                    ?? throw new Exception("JWT Key is missing in appsettings.json");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key))
                };

                // 🔥 CUSTOM 401 / 403 HANDLING (THIS IS WHAT YOU ASKED FOR)
                options.Events = new JwtBearerEvents
                {
                    // ❌ No token / invalid token
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            message = "You are not authenticated. Please login."
                        };

                        return context.Response.WriteAsync(
                            JsonSerializer.Serialize(response)
                        );
                    },

                    // ❌ Logged in but no permission (e.g. User accessing Admin API)
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            message = "You do not have permission to access this resource."
                        };

                        return context.Response.WriteAsync(
                            JsonSerializer.Serialize(response)
                        );
                    }
                };
            });

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using HyperPost.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HyperPost.Extensions
{
    public static class Auth
    {
        public static void AddHyperPostAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = AuthService.GetTokenValidationParameters();
                });
        }
    }
}

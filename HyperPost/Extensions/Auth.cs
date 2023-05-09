using Microsoft.Extensions.DependencyInjection;
using HyperPost.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HyperPost.Shared;

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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                {
                    policy.RequireClaim("Role", "admin");
                    policy.RequireClaim("RoleId", UserRolesEnum.Admin.ToString());
                });

                options.AddPolicy("manager", policy =>
                {
                    policy.RequireClaim("Role", "manager");
                    policy.RequireClaim("RoleId", UserRolesEnum.Manager.ToString());
                });

                options.AddPolicy("admin, manager", policy =>
                {
                    policy.RequireClaim("Role", "admin", "manager");
                    policy.RequireClaim("RoleId", UserRolesEnum.Admin.ToString(), UserRolesEnum.Manager.ToString());
                });

                options.AddPolicy("client", policy =>
                {
                    policy.RequireClaim("Role", "client");
                    policy.RequireClaim("RoleId", UserRolesEnum.Client.ToString());
                });
            });
        }
    }
}

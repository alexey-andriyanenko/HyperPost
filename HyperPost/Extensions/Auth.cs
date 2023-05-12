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
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = AuthService.GetTokenValidationParameters();
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "admin",
                    policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            var isAdmin = context.User.HasClaim(
                                c => c.Type == "Role" && c.Value == "admin"
                            );
                            var isAdminId = context.User.HasClaim(
                                c =>
                                    c.Type == "RoleId"
                                    && int.Parse(c.Value) == (int)UserRolesEnum.Admin
                            );

                            return isAdmin && isAdminId;
                        });
                    }
                );

                options.AddPolicy(
                    "manager",
                    policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            var isManager = context.User.HasClaim(
                                c => c.Type == "Role" && c.Value == "manager"
                            );
                            var isManagerId = context.User.HasClaim(
                                c =>
                                    c.Type == "RoleId"
                                    && int.Parse(c.Value) == (int)UserRolesEnum.Manager
                            );
                            return isManager && isManagerId;
                        });
                    }
                );

                options.AddPolicy(
                    "admin, manager",
                    policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            var isAdmin = context.User.HasClaim(
                                c => c.Type == "Role" && c.Value == "admin"
                            );
                            var isAdminId = context.User.HasClaim(
                                c =>
                                    c.Type == "RoleId"
                                    && int.Parse(c.Value) == (int)UserRolesEnum.Admin
                            );

                            var isManager = context.User.HasClaim(
                                c => c.Type == "Role" && c.Value == "manager"
                            );
                            var isManagerId = context.User.HasClaim(
                                c =>
                                    c.Type == "RoleId"
                                    && int.Parse(c.Value) == (int)UserRolesEnum.Manager
                            );

                            return (isAdmin && isAdminId) || (isManager && isManagerId);
                        });
                    }
                );

                options.AddPolicy(
                    "client",
                    policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            var isClient = context.User.HasClaim(
                                c => c.Type == "Role" && c.Value == "client"
                            );
                            var isClientId = context.User.HasClaim(
                                c =>
                                    c.Type == "RoleId"
                                    && int.Parse(c.Value) == (int)UserRolesEnum.Client
                            );
                            return isClient && isClientId;
                        });
                    }
                );
            });
        }
    }
}

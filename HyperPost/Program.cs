using HyperPost.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HyperPost.Extensions;

namespace HyperPost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<HyperPostDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("HyperPostTests"));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin();
                        policy.AllowAnyMethod();
                        policy.AllowAnyHeader();
                    }
                );
            });

            builder.Services.AddHyperPostAuthentication();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            builder.Services.AddHyperPostValidation();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            app.UseCors("AllowAll");
            app.Run();
        }
    }
}

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

            builder.Services.AddHyperPostAuthentication();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
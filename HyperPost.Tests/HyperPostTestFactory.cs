using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using HyperPost.DB;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace MyStore.Tests
{
    public class HyperPostTestFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<HyperPostDbContext>));
                services.Remove(dbContextDescriptor);
                services.AddDbContext<HyperPostDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("MyStoreConnection"));
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}
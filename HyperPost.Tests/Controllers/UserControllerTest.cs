using HyperPost.DB;
using HyperPost.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyStore.Tests;
using System.Net;
using System.Net.Http.Json;

namespace HyperPost.Tests.Controllers
{
    public class UserControllerTest : IClassFixture<HyperPostTestFactory<Program>>
    {
        private readonly HyperPostTestFactory<Program> _factory;
        private readonly HttpClient _client;
        public UserControllerTest(HyperPostTestFactory<Program> factory) 
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_CreateAdminUser_ReturnsCreated()
        {
            var user = UsersHelper.GetUserRequest(UsersEnum.Admin);
            var response = await _client.PostAsJsonAsync("/users", user);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Cleanup();
        }

        [Fact]
        public async Task POST_CreateManagerUser_ReturnsCreated()
        {
            var user = UsersHelper.GetUserRequest(UsersEnum.Manager);
            var response = await _client.PostAsJsonAsync("/users", user);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Cleanup();
        }

        [Fact]
        public async Task POST_CreateClientUser_ReturnsCreated()
        {
            var user = UsersHelper.GetUserRequest(UsersEnum.Client);
            var response = await _client.PostAsJsonAsync("/users", user);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Cleanup();
        }

        private void Cleanup()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                db.Users.ExecuteDelete();
            }
        }
    }
}

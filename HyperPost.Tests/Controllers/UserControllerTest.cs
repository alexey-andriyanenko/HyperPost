using HyperPost.DTO.User;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using HyperPost.DB;

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
        public async Task POST_AdminCreatesAdmin_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Admin,
                FirstName = "Admin_V2",
                LastName = "User",
                Email = "admin_v2@example.com",
                PhoneNumber = "1234567890",
                Password = "root_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, login.AccessToken);
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(u => u.PhoneNumber == "1234567890" && u.Email == "admin_v2@example.com");

                db.Users.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_AdminCreatesManager_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Manager,
                FirstName = "Manager_V2",
                LastName = "User",
                Email = "manager_v2@example.com",
                PhoneNumber = "1234567890",
                Password = "manager_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, login.AccessToken);
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(u => u.PhoneNumber == "1234567890" && u.Email == "manager_v2@example.com");

                db.Users.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_AdminCreatesClient_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_V2",
                LastName = "User",
                Email = "client_v2@example.com",
                PhoneNumber = "1234567890",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, login.AccessToken);
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(u => u.PhoneNumber == "1234567890" && u.Email == "client_v2@example.com");

                db.Users.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_ManagerCreatesAdmin_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Admin,
                FirstName = "Admin_V2",
                LastName = "User",
                Email = "admin_v2@example.com",
                PhoneNumber = "1234567890",
                Password = "admin_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, login.AccessToken);
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_ManagerCreatesManager_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Manager,
                FirstName = "Manager_V2",
                LastName = "User",
                Email = "manager_v2@example.com",
                PhoneNumber = "1234567890",
                Password = "manager_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, login.AccessToken);
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_ManagerCreatesClient_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_V2",
                LastName = "User",
                Email = "client_v2@example.com",
                PhoneNumber = "1234567890",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, login.AccessToken);
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(u => u.PhoneNumber == "1234567890" && u.Email == "client_v2@example.com");

                db.Users.Remove(model);
                db.SaveChanges();
            }
        }
    }
}

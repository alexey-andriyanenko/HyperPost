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
        public async Task POST_AnonymousCreatesUser_ReturnsUnauthorized()
        {
            var user = UsersHelper.GetUserRequest(ClientsEnum.Default);
            var response = await _client.PostAsJsonAsync("http://localhost:8000/users", user);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(content);
            Assert.Equal(user.RoleId, content.RoleId);
            Assert.Equal(user.FirstName, content.FirstName);
            Assert.Equal(user.LastName, content.LastName);
            Assert.Equal(user.Email, content.Email);
            Assert.Equal(user.PhoneNumber, content.PhoneNumber);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(
                    u => u.PhoneNumber == "1234567890" && u.Email == "admin_v2@example.com"
                );

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
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(content);
            Assert.Equal(user.RoleId, content.RoleId);
            Assert.Equal(user.FirstName, content.FirstName);
            Assert.Equal(user.LastName, content.LastName);
            Assert.Equal(user.Email, content.Email);
            Assert.Equal(user.PhoneNumber, content.PhoneNumber);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(
                    u => u.PhoneNumber == "1234567890" && u.Email == "manager_v2@example.com"
                );

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
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(content);
            Assert.Equal(user.RoleId, content.RoleId);
            Assert.Equal(user.FirstName, content.FirstName);
            Assert.Equal(user.LastName, content.LastName);
            Assert.Equal(user.Email, content.Email);
            Assert.Equal(user.PhoneNumber, content.PhoneNumber);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(
                    u => u.PhoneNumber == "1234567890" && u.Email == "client_v2@example.com"
                );

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
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
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
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
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
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(content);
            Assert.Equal(user.RoleId, content.RoleId);
            Assert.Equal(user.FirstName, content.FirstName);
            Assert.Equal(user.LastName, content.LastName);
            Assert.Equal(user.Email, content.Email);
            Assert.Equal(user.PhoneNumber, content.PhoneNumber);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Users.Single(
                    u => u.PhoneNumber == "1234567890" && u.Email == "client_v2@example.com"
                );

                db.Users.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreateUserWithExistingEmail_ReturnsBadRequest()
        {
            var existingClient = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_V2",
                LastName = "User",
                Email = existingClient.Email,
                PhoneNumber = "12345678",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithExistingPhoneNumber_ReturnsBadRequest()
        {
            var existingClient = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_V2",
                LastName = "User",
                Email = "client_v2@example.com",
                PhoneNumber = existingClient.PhoneNumber,
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithFirstNameMoreThan30Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "lllllllllllllllllllllllllllllll",
                LastName = "User",
                Email = "client_v2@example.com",
                PhoneNumber = "23142512",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithLastNameMoreThan30Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_v2",
                LastName = "lllllllllllllllllllllllllllllll",
                Email = "client_v2@example.com",
                PhoneNumber = "23142512",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithEmailMoreThan30Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_v2",
                LastName = "User",
                Email = "lllllllllllllllllllllllllllllll@mail.com",
                PhoneNumber = "23142512",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithInvalidEmail_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_v2",
                LastName = "User",
                Email = "client_v2",
                PhoneNumber = "23142512",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithPhoneNumberMoreThan20Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_v2",
                LastName = "User",
                Email = "email@mail.com",
                PhoneNumber = "123456789012345678901",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithInvalidUserRequest_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = null,
                LastName = null,
                Email = null,
                PhoneNumber = null,
                Password = null,
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateUserWithoutEmail_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_v2",
                LastName = "User",
                Email = null,
                PhoneNumber = "23142512",
                Password = "client_v2",
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var userToDelete = context.Users.Single(u => u.PhoneNumber == "23142512");

                context.Users.Remove(userToDelete);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task POST_CreateClientUserWithoutPassword_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Client_v2",
                LastName = "User",
                Email = "example@.com",
                PhoneNumber = "23142512",
                Password = null,
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(content);
            Assert.Equal(user.RoleId, content.RoleId);
            Assert.Equal(user.FirstName, content.FirstName);
            Assert.Equal(user.LastName, content.LastName);
            Assert.Equal(user.Email, content.Email);
            Assert.Equal(user.PhoneNumber, content.PhoneNumber);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var userToDelete = context.Users.Single(u => u.PhoneNumber == "23142512");
                context.Users.Remove(userToDelete);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task POST_CreateAdminUserWithoutPassword_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Admin,
                FirstName = "Admin_v2",
                LastName = "User",
                Email = "example@.com",
                PhoneNumber = "23142512",
                Password = null,
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreateManagerUserWithoutPassword_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var user = new UserRequest
            {
                RoleId = (int)UserRolesEnum.Manager,
                FirstName = "Manager_v2",
                LastName = "User",
                Email = "example@.com",
                PhoneNumber = "23142512",
                Password = null,
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(user);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/users");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GET_AnonymousGetsUserById_ReturnsUnauthorized()
        {
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://localhost:8000/users/1");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsUserById_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var existingClient = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );

            message.RequestUri = new Uri($"http://localhost:8000/users/{existingClient.Id}");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.Equal(existingClient.Id, user.Id);
            Assert.Equal(existingClient.RoleId, user.RoleId);
            Assert.Equal(existingClient.FirstName, user.FirstName);
            Assert.Equal(existingClient.LastName, user.LastName);
            Assert.Equal(existingClient.Email, user.Email);
            Assert.Equal(existingClient.PhoneNumber, user.PhoneNumber);
        }

        [Fact]
        public async Task GET_ManagerGetsUserById_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var existingClient = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/users/{existingClient.Id}");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.Equal(existingClient.Id, user.Id);
            Assert.Equal(existingClient.RoleId, user.RoleId);
            Assert.Equal(existingClient.FirstName, user.FirstName);
            Assert.Equal(existingClient.LastName, user.LastName);
            Assert.Equal(existingClient.Email, user.Email);
            Assert.Equal(existingClient.PhoneNumber, user.PhoneNumber);
        }

        [Fact]
        public async Task GET_ClientGetsUserById_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var existingClient = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/users/{existingClient.Id}");
            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsUserById_ReturnsNotFound()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/users/0");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

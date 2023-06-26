using HyperPost.DTO.User;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using HyperPost.DB;
using HyperPost.Migrations;
using HyperPost.DTO.Pagination;
using Microsoft.AspNetCore.WebUtilities;

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
            var user = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var response = await _client.PostAsJsonAsync("http://localhost:8000/users", user);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task POST_AdminCreatesAdmin_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            var user = new CreateUserRequest
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

            var user = new CreateUserRequest
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

            var user = new CreateUserRequest
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

            var user = new CreateUserRequest
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

            var user = new CreateUserRequest
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

            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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
            var user = new CreateUserRequest
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

        [Fact]
        public async Task PUT_AnonymousUpdatesUser_ReturnsUnauthorized()
        {
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Put;
            message.Content = JsonContent.Create(
                new CreateUserRequest
                {
                    RoleId = (int)UserRolesEnum.Client,
                    FirstName = "Client_v2",
                    LastName = "User",
                    Email = "example@.com",
                    PhoneNumber = "23142512",
                    Password = "123456",
                }
            );
            message.RequestUri = new Uri("http://localhost:8000/users/1");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PUT_AdminUpdatesAdmin_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Admin);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = UsersHelper.GetUserRequest(TestUsersEnum.Admin);
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = context.Users.Single(u => u.Id == postContent.Id);
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_AdminUpdatesManager_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Manager);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = UsersHelper.GetUserRequest(TestUsersEnum.Manager);
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = context.Users.Single(u => u.Id == postContent.Id);
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_AdminUpdatesClient_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = postUser.RoleId,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(postContent.Id, putContent.Id);
            Assert.Equal(postUser.RoleId, putContent.RoleId);
            Assert.Equal(putUser.FirstName, putContent.FirstName);
            Assert.Equal(putUser.LastName, putContent.LastName);
            Assert.Equal(putUser.Email, putContent.Email);
            Assert.Equal(postUser.PhoneNumber, putContent.PhoneNumber);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ManagerUpdatesAdmin_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Admin);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Admin,
                FirstName = "Updated Admin FirstName",
                LastName = "Updated Admin LastName",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ManagerUpdatesManager_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Manager);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Manager,
                FirstName = "Updated Manager FirstName",
                LastName = "Updated Manager LastName",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ManagerUpdatesClient_ReturnsOk()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(postContent.Id, putContent.Id);
            Assert.Equal(postUser.RoleId, putContent.RoleId);
            Assert.Equal(putUser.FirstName, putContent.FirstName);
            Assert.Equal(putUser.LastName, putContent.LastName);
            Assert.Equal(putUser.Email, putContent.Email);
            Assert.Equal(postUser.PhoneNumber, putContent.PhoneNumber);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ClientUpdatesUser_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesUserWithEmptyData_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();
            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "",
                LastName = "",
                Email = "",
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesUserWithFirstNameMoreThan30Characters_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "1234567890123456789012345678901",
                LastName = "Updated Client LastName",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesUserWithLastNameMoreThan30Characters_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postResponse = await _client.SendAsync(postMessage);

            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "1234567890123456789012345678901",
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_SetsEmail_ReturnsOk()
        {
            /* Test Description
             * user should be able to update his email if it was not set before
             */

            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            postUser.Email = null;

            var postMessage = new HttpRequestMessage();
            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = "temp@example.com",
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(putUser.Email, putContent.Email);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ChangesEmail_ReturnsBadRequest()
        {
            /* Test Description
             * user should not be able to change his email if it was set before
             */

            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = "new@email.com",
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_SetsEmailWithMoreThan50Characters_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            postUser.Email = null;

            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = "llllllllllllllllllllllllllllllllllllllllllllllllll@email.com",
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_SetsInvalidEmail_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            postUser.Email = null;

            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = "invalidemail",
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_SetsExistingEmail_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var existingUser = UsersHelper.GetExistingUserModel(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            postUser.Email = null;

            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // update user ↓
            var putUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            putUser.Email = existingUser.Email;

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesNonExistentUser_ReturnsNotFound()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // update user ↓
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = "some@example.com",
            };

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/0");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
            // update user ↑
        }

        [Fact]
        public async Task PUT_AnonymousUpdatesHimself_ReturnsUnauthorized()
        {
            var putUser = new UpdateUserRequest
            {
                RoleId = (int)UserRolesEnum.Client,
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = "some@email.com",
            };

            var response = await _client.PutAsJsonAsync($"http://localhost:8000/users/me", putUser);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PUT_UserUpdateHimself_ReturnsOK()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);

            // create user ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // login as new user ↓
            var loginMessage = new HttpRequestMessage();

            loginMessage.Method = HttpMethod.Post;
            loginMessage.Content = JsonContent.Create(
                new UserLoginViaEmailRequest
                {
                    Email = postUser.Email,
                    Password = postUser.Password,
                }
            );
            loginMessage.RequestUri = new Uri("http://localhost:8000/users/login/email");

            var loginResponse = await _client.SendAsync(loginMessage);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(loginContent);
            // login as new user ↑

            // update user ↓
            var putUser = new UpdateMeRequest
            {
                FirstName = "Updated Client FirstName",
                LastName = "Updated Client LastName",
                Email = postUser.Email,
            };

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                loginContent.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/me");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(putUser.FirstName, putContent.FirstName);
            Assert.Equal(putUser.LastName, putContent.LastName);
            Assert.Equal(putUser.Email, putContent.Email);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UserUpdatesHimselfWithEmptyData_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);

            // create user ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // login as new user ↓
            var loginMessage = new HttpRequestMessage();

            loginMessage.Method = HttpMethod.Post;
            loginMessage.Content = JsonContent.Create(
                new UserLoginViaEmailRequest
                {
                    Email = postUser.Email,
                    Password = postUser.Password,
                }
            );
            loginMessage.RequestUri = new Uri("http://localhost:8000/users/login/email");

            var loginResponse = await _client.SendAsync(loginMessage);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(loginContent);
            // login as new user ↑

            // update user ↓
            var putUser = new UpdateMeRequest
            {
                FirstName = "",
                LastName = "",
                Email = "",
            };

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                loginContent.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/me");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UserUpdatesHimSelfWithFirstNameMoreThan30Characters_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);

            // create user ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // login as new user ↓
            var loginMessage = new HttpRequestMessage();

            loginMessage.Method = HttpMethod.Post;
            loginMessage.Content = JsonContent.Create(
                new UserLoginViaEmailRequest
                {
                    Email = postUser.Email,
                    Password = postUser.Password,
                }
            );
            loginMessage.RequestUri = new Uri("http://localhost:8000/users/login/email");

            var loginResponse = await _client.SendAsync(loginMessage);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(loginContent);
            // login as new user ↑

            // update user ↓
            var putUser = new UpdateMeRequest
            {
                FirstName = "very long first name very long first name very long first name",
                LastName = postUser.LastName,
                Email = postUser.Email,
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                loginContent.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/me");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UserUpdatesHimSelfWithLastNameMoreThan30Characters_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);

            // create user ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // login as new user ↓
            var loginMessage = new HttpRequestMessage();

            loginMessage.Method = HttpMethod.Post;
            loginMessage.Content = JsonContent.Create(
                new UserLoginViaEmailRequest
                {
                    Email = postUser.Email,
                    Password = postUser.Password,
                }
            );
            loginMessage.RequestUri = new Uri("http://localhost:8000/users/login/email");

            var loginResponse = await _client.SendAsync(loginMessage);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(loginContent);
            // login as new user ↑

            // update user ↓
            var putUser = new UpdateMeRequest
            {
                FirstName = postUser.FirstName,
                LastName = "very long last name very long last name very long last name",
                Email = postUser.Email,
            };

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                loginContent.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/me");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UserUpdateHimSelf_SetsEmail_ReturnsOk()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            postUser.Email = null;

            // create user ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // login as new user ↓
            var loginMessage = new HttpRequestMessage();

            loginMessage.Method = HttpMethod.Post;
            loginMessage.Content = JsonContent.Create(
                new UserLoginViaPhoneNumberRequest
                {
                    PhoneNumber = postUser.PhoneNumber,
                    Password = postUser.Password,
                }
            );
            loginMessage.RequestUri = new Uri("http://localhost:8000/users/login/phone");

            var loginResponse = await _client.SendAsync(loginMessage);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(loginContent);
            // login as new user ↑

            // update user ↓
            var putUser = new UpdateMeRequest
            {
                FirstName = postUser.FirstName,
                LastName = postUser.LastName,
                Email = "valid@example.com",
            };

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                loginContent.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/me");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(putUser.Email, putContent.Email);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public async Task PUT_UserUpdateHimSelf_ChangesEmail_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);

            // create user ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // login as new user ↓
            var loginMessage = new HttpRequestMessage();

            loginMessage.Method = HttpMethod.Post;
            loginMessage.Content = JsonContent.Create(
                new UserLoginViaPhoneNumberRequest
                {
                    PhoneNumber = postUser.PhoneNumber,
                    Password = postUser.Password,
                }
            );
            loginMessage.RequestUri = new Uri("http://localhost:8000/users/login/phone");

            var loginResponse = await _client.SendAsync(loginMessage);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(loginContent);
            // login as new user ↑

            // update user ↓
            var putUser = new UpdateMeRequest
            {
                FirstName = postUser.FirstName,
                LastName = postUser.LastName,
                Email = "invalid",
            };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putUser);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                loginContent.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/users/me");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update user ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = db.Users.Single(u => u.Id == postContent.Id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_AnonymousDeletesUser_ReturnsUnauthorized()
        {
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/0");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task DELETE_ClientDeletesUser_ReturnsForbidden()
        {
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);

            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/0");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task DELETE_AdminDeletesAdmin_ReturnsNoContent()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Admin);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // delete user ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            // delete user ↑
        }

        [Fact]
        public async Task DELETE_AdminDeletesManager_ReturnsNoContent()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Manager);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // delete user ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            // delete user ↑
        }

        [Fact]
        public async Task DELETE_AdminDeletesClient_ReturnsNoContent()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // delete user ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            // delete user ↑
        }

        [Fact]
        public async Task DELETE_ManagerDeletesAdmin_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Admin);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // delete user ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            // delete user ↑

            // cleanup ↓
            using (var scrope = _factory.Services.CreateScope())
            {
                var context = scrope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = context.Users.Single(u => u.Id == postContent.Id);
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_ManagerDeletesManager_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.Manager);
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // delete user ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            // delete user ↑

            // cleanup ↓
            using (var scrope = _factory.Services.CreateScope())
            {
                var context = scrope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var user = context.Users.Single(u => u.Id == postContent.Id);
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_ManagerDeletesClient_ReturnsNoContent()
        {
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create user ↓
            var postUser = UsersHelper.GetUserRequest(TestUsersEnum.DefaultClient);
            var postMessage = new HttpRequestMessage();
            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postUser);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postContent);
            // create user ↑

            // delete user ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{postContent.Id}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            // delete user ↑
        }

        [Fact]
        public async Task DELETE_DeletesNonExistentUser_ReturnsNotFound()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri($"http://localhost:8000/users/{Guid.NewGuid()}");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AnonymouesGetsUsers_ReturnsUnauthorized()
        {
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.RequestUri = new Uri("http://localhost:8000/users");

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsUsers_ReturnsOkAllUsers()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/users",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            getMessage.RequestUri = new Uri(url);

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadFromJsonAsync<
                PaginationResponse<UserResponse>
            >();
            Assert.NotNull(getContent);
            Assert.Contains(getContent.List, u => u.RoleId == (int)UserRolesEnum.Admin);
            Assert.Contains(getContent.List, u => u.RoleId == (int)UserRolesEnum.Manager);
            Assert.Contains(getContent.List, u => u.RoleId == (int)UserRolesEnum.Client);

            Assert.Equal(1, getContent.TotalPages);
            Assert.Equal(3, getContent.TotalCount);
        }

        [Fact]
        public async Task GET_ManagerGetsUsers_ReturnsOkOnlyClients()
        {
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/users",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            getMessage.RequestUri = new Uri(url);

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadFromJsonAsync<
                PaginationResponse<UserResponse>
            >();
            Assert.NotNull(getContent);
            Assert.DoesNotContain(getContent.List, u => u.RoleId == (int)UserRolesEnum.Admin);
            Assert.DoesNotContain(getContent.List, u => u.RoleId == (int)UserRolesEnum.Manager);
            Assert.Contains(getContent.List, u => u.RoleId == (int)UserRolesEnum.Client);
            Assert.Equal(1, getContent.TotalPages);
            Assert.Equal(1, getContent.TotalCount);
        }

        [Fact]
        public async Task GET_ClientGetsUsers_ReturnsForbidden()
        {
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/users",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            getMessage.RequestUri = new Uri(url);

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.Forbidden, getResponse.StatusCode);
        }

        [Fact]
        public async Task GET_GetsUsersWithInvalidParams_ReturnsBadRequest()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 0, Limit = 0 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/users",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            getMessage.RequestUri = new Uri(url);

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AnonymousGetsHimself_ReturnsUnauthorized()
        {
            var getMessage = new HttpRequestMessage();
            getMessage.Method = HttpMethod.Get;
            getMessage.RequestUri = new Uri("http://localhost:8000/users/me");
            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetssHimself_ReturnsOk()
        {
            var adminUser = UsersHelper.GetExistingUserModel(UserRolesEnum.Admin);
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            getMessage.RequestUri = new Uri("http://localhost:8000/users/me");

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(getContent);
            Assert.Equal(adminLogin.Id, getContent.Id);
            Assert.Equal(adminUser.Email, getContent.Email);
            Assert.Equal(adminUser.PhoneNumber, getContent.PhoneNumber);
            Assert.Equal(adminUser.FirstName, getContent.FirstName);
            Assert.Equal(adminUser.LastName, getContent.LastName);
            Assert.Equal(adminUser.RoleId, getContent.RoleId);
        }

        [Fact]
        public async Task GET_ManagerGetsHimself_ReturnsOk()
        {
            var managerUser = UsersHelper.GetExistingUserModel(UserRolesEnum.Manager);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            getMessage.RequestUri = new Uri("http://localhost:8000/users/me");

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(getContent);
            Assert.Equal(managerLogin.Id, getContent.Id);
            Assert.Equal(managerUser.Email, getContent.Email);
            Assert.Equal(managerUser.PhoneNumber, getContent.PhoneNumber);
            Assert.Equal(managerUser.FirstName, getContent.FirstName);
            Assert.Equal(managerUser.LastName, getContent.LastName);
            Assert.Equal(managerUser.RoleId, getContent.RoleId);
        }

        [Fact]
        public async Task GET_ClientGetsHimself_ReturnsOk()
        {
            var clientUser = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);

            var getMessage = new HttpRequestMessage();
            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            getMessage.RequestUri = new Uri("http://localhost:8000/users/me");

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(getContent);
            Assert.Equal(clientLogin.Id, getContent.Id);
            Assert.Equal(clientUser.Email, getContent.Email);
            Assert.Equal(clientUser.PhoneNumber, getContent.PhoneNumber);
            Assert.Equal(clientUser.FirstName, getContent.FirstName);
            Assert.Equal(clientUser.LastName, getContent.LastName);
            Assert.Equal(clientUser.RoleId, getContent.RoleId);
        }
    }
}

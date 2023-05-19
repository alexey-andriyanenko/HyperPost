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
        public async Task PUT_AdminUpdatesUser_ReturnsOk()
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
    }
}

using HyperPost.DB;
using HyperPost.DTO.Package;
using HyperPost.DTO.Pagination;
using HyperPost.DTO.User;
using HyperPost.Migrations;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HyperPost.Tests.Controllers
{
    public class PackagesControllerTest : IClassFixture<HyperPostTestFactory<Program>>
    {
        private readonly HyperPostTestFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public PackagesControllerTest(HyperPostTestFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task POST_AdminCreatesPackage_ReturnsCreated()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);

            // create sender ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender ↑

            // create receiver ↓
            var recieverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Content = JsonContent.Create(recieverUser);
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(9);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);

            Assert.Equal((int)PackageStatusesEnum.Created, postPackageContent.StatusId);
            Assert.Equal(package.CategoryId, postPackageContent.Category.Id);
            Assert.Equal(package.SenderUserId, postPackageContent.SenderUser.Id);
            Assert.Equal(package.ReceiverUserId, postPackageContent.ReceiverUser.Id);
            Assert.Equal(package.SenderDepartmentId, postPackageContent.SenderDepartment.Id);
            Assert.Equal(package.ReceiverDepartmentId, postPackageContent.ReceiverDepartment.Id);
            Assert.Equal(package.Weight, postPackageContent.Weight);
            Assert.Equal(package.PackagePrice, postPackageContent.PackagePrice);
            Assert.Equal(package.DeliveryPrice, postPackageContent.DeliveryPrice);
            Assert.Equal(package.Description, postPackageContent.Description);

            Assert.Null(postPackageContent.ModifiedAt);
            Assert.Null(postPackageContent.SentAt);
            Assert.Null(postPackageContent.ArrivedAt);
            Assert.Null(postPackageContent.ReceivedAt);
            Assert.Null(postPackageContent.ArchivedAt);
            // create package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);

                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_ManagerCreatesPackage_ReturnsCreated()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // createsender ↑

            // create receiver ↓
            var recieverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Content = JsonContent.Create(recieverUser);
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(9);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();
            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);

            Assert.Equal((int)PackageStatusesEnum.Created, postPackageContent.StatusId);
            Assert.Equal(package.CategoryId, postPackageContent.Category.Id);
            Assert.Equal(package.SenderUserId, postPackageContent.SenderUser.Id);
            Assert.Equal(package.ReceiverUserId, postPackageContent.ReceiverUser.Id);
            Assert.Equal(package.SenderDepartmentId, postPackageContent.SenderDepartment.Id);
            Assert.Equal(package.ReceiverDepartmentId, postPackageContent.ReceiverDepartment.Id);
            Assert.Equal(package.Weight, postPackageContent.Weight);
            Assert.Equal(package.PackagePrice, postPackageContent.PackagePrice);
            Assert.Equal(package.DeliveryPrice, postPackageContent.DeliveryPrice);
            Assert.Equal(package.Description, postPackageContent.Description);

            Assert.Null(postPackageContent.ModifiedAt);
            Assert.Null(postPackageContent.SentAt);
            Assert.Null(postPackageContent.ArrivedAt);
            Assert.Null(postPackageContent.ReceivedAt);
            Assert.Null(postPackageContent.ArchivedAt);
            // create package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);

                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_ClientCreatesPackage_ReturnsForbidden()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = 55,
                ReceiverUserId = 56,
                SenderDepartmentId = 57,
                ReceiverDepartmentId = 58,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreatesPackageWithInvalidData_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var package = new CreatePackageRequest();

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreatesPackageWithNonExistentCategory_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender ↑

            // create receiver ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver ↑

            var senderDepartmend = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = 0,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartmend.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);

                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithNonExistentSenderUser_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = 0,
                ReceiverUserId = 56,
                SenderDepartmentId = 57,
                ReceiverDepartmentId = 58,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_CreatesPackageWithNonExistentReceiverUser_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);

            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = 0,
                SenderDepartmentId = 57,
                ReceiverDepartmentId = 58,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithEqualSenderAndReceiver_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();
            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);
            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postSenderUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithEqualSenderAndReceiverDepartments_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();
            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);
            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();
            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);
            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = senderDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);

                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithNonExistentSenderDepartment_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);

            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = 0,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);

                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithNonExistentReceiverDepartment_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();
            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);
            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();
            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);
            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = 0,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with price that violates decimal(8, 2) constraint
        [Fact]
        public async Task POST_CreatesPackageWithPackagePriceMoreThan8Digits_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100000000.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with price with more than 2 digits after decimal point
        [Fact]
        public async Task POST_CreatesPackageWithPackagePriceMoreThan2DigitsAfterDecimalPoint_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.555m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with delivery price that violates decimal(8, 2) constraint
        [Fact]
        public async Task POST_CreatesPackageWithDeliveryPriceMoreThan8Digits_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 100000000.5m,
                Description = "Test package"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with delivery price with more than 2 digits after decimal point
        [Fact]
        public async Task POST_CreatesPackageWithDeliveryPriceMoreThan2DigitsAfterDecimalPoint_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓

            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();
            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.555m,
                Description = "Test package"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");
            var response = await _httpClient.SendAsync(message);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithDeliveryPriceLessThan5_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();
            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);
            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 4.99m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with weight that viloates decimal(4, 2) constraint
        [Fact]
        public async Task POST_CreatesPackageWithWeightMoreThan4Digits_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 100.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = "Test package"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");
            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with weight with more than 2 digits after decimal point
        [Fact]
        public async Task POST_CreatesPackageWithWeightMoreThan2DigitsAfterDecimalPoint_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.555m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        // creates package with less than 0.2 weight
        [Fact]
        public async Task POST_CreatesPackageWithLessThan02Weight_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 0.1m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = "Test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(package);
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_CreatesPackageWithDescriptionMoreThan50Characters_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();
            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);
            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑
            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();
            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);
            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑
            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.555m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = new string('d', 51),
            };
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(receiverDepartment);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/packages");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_AnonymousGetsPackage_ReturnsUnauthorized()
        {
            var guid = Guid.NewGuid();
            var response = await _httpClient.GetAsync($"http://localhost:8000/packages/{guid}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsPackage_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.1m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = "test description"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // get package ↓
            var getPackageMessage = new HttpRequestMessage();

            getPackageMessage.Method = HttpMethod.Get;
            getPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}"
            );

            var getPackageResponse = await _httpClient.SendAsync(getPackageMessage);
            Assert.Equal(HttpStatusCode.OK, getPackageResponse.StatusCode);

            var getPackageContent =
                await getPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(getPackageContent);
            Assert.Equal(postPackageContent.Id, getPackageContent.Id);
            Assert.Equal(postPackageContent.Category.Id, getPackageContent.Category.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, getPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, getPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                getPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                getPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, getPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, getPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, getPackageContent.DeliveryPrice);
            Assert.Equal(postPackageContent.StatusId, getPackageContent.StatusId);
            Assert.Equal(postPackageContent.CreatedAt, getPackageContent.CreatedAt);
            Assert.Equal(postPackageContent.ModifiedAt, getPackageContent.ModifiedAt);
            Assert.Equal(postPackageContent.ArrivedAt, getPackageContent.ArrivedAt);
            Assert.Equal(postPackageContent.ReceivedAt, getPackageContent.ReceivedAt);
            Assert.Equal(postPackageContent.ArchivedAt, getPackageContent.ArchivedAt);
            Assert.Equal(postPackageContent.Description, getPackageContent.Description);
            // get package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_ManagerGetsPackage_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.1m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // get package ↓
            var getPackageMessage = new HttpRequestMessage();

            getPackageMessage.Method = HttpMethod.Get;
            getPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}"
            );

            var getPackageResponse = await _httpClient.SendAsync(getPackageMessage);
            Assert.Equal(HttpStatusCode.OK, getPackageResponse.StatusCode);

            var getPackageContent =
                await getPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(getPackageContent);
            Assert.Equal(postPackageContent.Id, getPackageContent.Id);
            Assert.Equal(postPackageContent.Category.Id, getPackageContent.Category.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, getPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, getPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                getPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                getPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, getPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, getPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, getPackageContent.DeliveryPrice);
            Assert.Equal(postPackageContent.StatusId, getPackageContent.StatusId);
            Assert.Equal(postPackageContent.CreatedAt, getPackageContent.CreatedAt);
            Assert.Equal(postPackageContent.ModifiedAt, getPackageContent.ModifiedAt);
            Assert.Equal(postPackageContent.ArrivedAt, getPackageContent.ArrivedAt);
            Assert.Equal(postPackageContent.ReceivedAt, getPackageContent.ReceivedAt);
            Assert.Equal(postPackageContent.ArchivedAt, getPackageContent.ArchivedAt);
            Assert.Equal(postPackageContent.Description, getPackageContent.Description);
            // get package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_ClientGetsPackage_ReturnsOk()
        {
            var managerLogin = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var clientLogin = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.1m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // get package ↓
            var getPackageMessage = new HttpRequestMessage();

            getPackageMessage.Method = HttpMethod.Get;
            getPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            getPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}"
            );

            var getPackageResponse = await _httpClient.SendAsync(getPackageMessage);
            Assert.Equal(HttpStatusCode.OK, getPackageResponse.StatusCode);

            var getPackageContent =
                await getPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(getPackageContent);
            Assert.Equal(postPackageContent.Id, getPackageContent.Id);
            Assert.Equal(postPackageContent.Category.Id, getPackageContent.Category.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, getPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, getPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                getPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                getPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, getPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, getPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, getPackageContent.DeliveryPrice);
            Assert.Equal(postPackageContent.StatusId, getPackageContent.StatusId);
            Assert.Equal(postPackageContent.CreatedAt, getPackageContent.CreatedAt);
            Assert.Equal(postPackageContent.ModifiedAt, getPackageContent.ModifiedAt);
            Assert.Equal(postPackageContent.ArrivedAt, getPackageContent.ArrivedAt);
            Assert.Equal(postPackageContent.ReceivedAt, getPackageContent.ReceivedAt);
            Assert.Equal(postPackageContent.ArchivedAt, getPackageContent.ArchivedAt);
            Assert.Equal(postPackageContent.Description, getPackageContent.Description);
            // get package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_GetsNonExistentPackage_ReturnsNotFound()
        {
            var guid = Guid.NewGuid();
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);

            var getPackageMessage = new HttpRequestMessage();
            getPackageMessage.Method = HttpMethod.Get;
            getPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackageMessage.RequestUri = new Uri($"http://localhost:8000/packages/{guid}");

            var getPackageResponse = await _httpClient.SendAsync(getPackageMessage);
            Assert.Equal(HttpStatusCode.NotFound, getPackageResponse.StatusCode);
        }

        [Fact]
        public async Task PUT_AnonymousUpdatesPackage_ReturnsUnauthorized()
        {
            var guid = Guid.NewGuid();
            var putPackageMessage = new HttpRequestMessage();
            putPackageMessage.Method = HttpMethod.Put;
            putPackageMessage.RequestUri = new Uri($"http://localhost:8000/packages/{guid}");

            var putPackageResponse = await _httpClient.SendAsync(putPackageMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, putPackageResponse.StatusCode);
        }

        [Fact]
        public async Task PUT_AdminUpdatesPackage_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var postPackage = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 2m,
                PackagePrice = 10m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(postPackage);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // update package ↓
            var putPackage = new UpdatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Clothes,
                Description = "Updated test package"
            };
            var putPackageMessage = new HttpRequestMessage();

            putPackageMessage.Method = HttpMethod.Put;
            putPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putPackageMessage.Content = JsonContent.Create(putPackage);
            putPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}"
            );

            var putPackageResponse = await _httpClient.SendAsync(putPackageMessage);
            Assert.Equal(HttpStatusCode.OK, putPackageResponse.StatusCode);

            var putPackageContent =
                await putPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(putPackageContent);
            // assert that valid response is returned
            Assert.Equal(postPackageContent.Id, putPackageContent.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, putPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, putPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                putPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                putPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, putPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, putPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, putPackageContent.DeliveryPrice);

            Assert.Equal(putPackage.CategoryId, putPackageContent.Category.Id);
            Assert.Equal(putPackage.Description, putPackageContent.Description);
            Assert.Equal((int)PackageStatusesEnum.Modified, putPackageContent.StatusId);

            Assert.NotNull(putPackageContent.CreatedAt);
            Assert.NotNull(putPackageContent.ModifiedAt);
            Assert.Null(putPackageContent.ArrivedAt);
            Assert.Null(putPackageContent.ReceivedAt);
            Assert.Null(putPackageContent.ArchivedAt);
            // update package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ManagerUpdatesPackage_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var postPackage = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 2m,
                PackagePrice = 10m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Content = JsonContent.Create(postPackage);
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // update package ↓
            var putPackage = new UpdatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Clothes,
                Description = "Updated test package"
            };
            var putPackageMessage = new HttpRequestMessage();

            putPackageMessage.Method = HttpMethod.Put;
            putPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putPackageMessage.Content = JsonContent.Create(putPackage);
            putPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}"
            );

            var putPackageResponse = await _httpClient.SendAsync(putPackageMessage);
            Assert.Equal(HttpStatusCode.OK, putPackageResponse.StatusCode);

            var putPackageContent =
                await putPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(putPackageContent);
            Assert.Equal(postPackageContent.Id, putPackageContent.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, putPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, putPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                putPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                putPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, putPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, putPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, putPackageContent.DeliveryPrice);

            Assert.Equal(putPackage.CategoryId, putPackageContent.Category.Id);
            Assert.Equal(putPackage.Description, putPackageContent.Description);
            Assert.Equal((int)PackageStatusesEnum.Modified, putPackageContent.StatusId);

            Assert.NotNull(putPackageContent.CreatedAt);
            Assert.NotNull(putPackageContent.ModifiedAt);
            Assert.Null(putPackageContent.ArrivedAt);
            Assert.Null(putPackageContent.ReceivedAt);
            Assert.Null(putPackageContent.ArchivedAt);
            // update package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ClientUpdatesPackage_ReturnsForbidden()
        {
            var guid = Guid.NewGuid();
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);
            var putPackage = new UpdatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Clothes,
                Description = "Updated test package"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Put;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(putPackage);
            message.RequestUri = new Uri($"http://localhost:8000/packages/{guid}");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task PUT_UpdatesPackageWithDescriptionMoreThan50Characters_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var putPackage = new UpdatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Clothes,
                Description = "Updated test package with description more than 50 characters"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Put;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(putPackage);
            message.RequestUri = new Uri($"http://localhost:8000/packages/{Guid.NewGuid()}");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PUT_UpdatesPackageWithNonExistentCategory_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var postSenderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Content = JsonContent.Create(postSenderUser);
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var postReceiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Content = JsonContent.Create(postReceiverUser);
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var postPackage = new CreatePackageRequest
            {
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 5m,
                PackagePrice = 5m,
                DeliveryPrice = 10m,
                CategoryId = (int)PackageCategoriesEnum.Clothes,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Content = JsonContent.Create(postPackage);
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // update package ↓
            var putPackage = new UpdatePackageRequest
            {
                CategoryId = 0,
                Description = "Updated test package"
            };
            var putPackageMessage = new HttpRequestMessage();

            putPackageMessage.Method = HttpMethod.Put;
            putPackageMessage.Content = JsonContent.Create(putPackage);
            putPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}"
            );

            var putPackageResponse = await _httpClient.SendAsync(putPackageMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putPackageResponse.StatusCode);
            // update package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesNonExistentPackage_ReturnsNotFound()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var putPackage = new UpdatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Clothes,
                Description = "Updated test package"
            };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Put;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.Content = JsonContent.Create(putPackage);
            message.RequestUri = new Uri($"http://localhost:8000/packages/{Guid.NewGuid()}");

            var response = await _httpClient.SendAsync(message);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_AnonymousArchivesPackage_ReturnsUnauthorized()
        {
            var response = await _httpClient.PatchAsync(
                $"http://localhost:8000/packages/{Guid.NewGuid()}/archive",
                null
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_AdminArchivesPackage_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);

            // create sender ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender ↑

            // create receiver ↓
            var recieverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Content = JsonContent.Create(recieverUser);
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(9);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };

            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // archive package ↓
            var patchPackageMessage = new HttpRequestMessage();

            patchPackageMessage.Method = HttpMethod.Patch;
            patchPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            patchPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}/archive"
            );

            var patchPackageResponse = await _httpClient.SendAsync(patchPackageMessage);
            Assert.Equal(HttpStatusCode.OK, patchPackageResponse.StatusCode);

            var patchPackageContent =
                await patchPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(patchPackageContent);
            Assert.Equal(postPackageContent.Id, patchPackageContent.Id);
            Assert.Equal(postPackageContent.Category.Id, patchPackageContent.Category.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, patchPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, patchPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                patchPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                patchPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, patchPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, patchPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, patchPackageContent.DeliveryPrice);
            Assert.Equal(postPackageContent.Description, patchPackageContent.Description);
            Assert.Equal(postPackageContent.CreatedAt, patchPackageContent.CreatedAt);
            Assert.Equal(postPackageContent.ModifiedAt, patchPackageContent.ModifiedAt);
            Assert.Equal(postPackageContent.ArrivedAt, postPackageContent.ArrivedAt);
            Assert.Equal(postPackageContent.SentAt, postPackageContent.SentAt);
            Assert.Equal(postPackageContent.ReceivedAt, postPackageContent.ReceivedAt);

            Assert.Equal((int)PackageStatusesEnum.Archived, patchPackageContent.StatusId);
            Assert.NotNull(patchPackageContent.ArchivedAt);
            // archive package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();

                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);

                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);

                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);

                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PATCH_ManagerArchivesPackage_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Content = JsonContent.Create(senderUser);
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender ↑

            // create receiver ↓
            var recieverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Content = JsonContent.Create(recieverUser);
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(9);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
                Description = "Test package"
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.Content = JsonContent.Create(package);

            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // archive package ↓
            var patchPackageMessage = new HttpRequestMessage();

            patchPackageMessage.Method = HttpMethod.Patch;
            patchPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            patchPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{postPackageContent.Id}/archive"
            );

            var patchPackageResponse = await _httpClient.SendAsync(patchPackageMessage);
            Assert.Equal(HttpStatusCode.OK, patchPackageResponse.StatusCode);

            var patchPackageContent =
                await patchPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(patchPackageContent);
            Assert.Equal(postPackageContent.Id, patchPackageContent.Id);
            Assert.Equal(postPackageContent.Category.Id, patchPackageContent.Category.Id);
            Assert.Equal(postPackageContent.SenderUser.Id, patchPackageContent.SenderUser.Id);
            Assert.Equal(postPackageContent.ReceiverUser.Id, patchPackageContent.ReceiverUser.Id);
            Assert.Equal(
                postPackageContent.SenderDepartment.Id,
                patchPackageContent.SenderDepartment.Id
            );
            Assert.Equal(
                postPackageContent.ReceiverDepartment.Id,
                patchPackageContent.ReceiverDepartment.Id
            );
            Assert.Equal(postPackageContent.Weight, patchPackageContent.Weight);
            Assert.Equal(postPackageContent.PackagePrice, patchPackageContent.PackagePrice);
            Assert.Equal(postPackageContent.DeliveryPrice, patchPackageContent.DeliveryPrice);
            Assert.Equal(postPackageContent.Description, patchPackageContent.Description);
            Assert.Equal(postPackageContent.CreatedAt, patchPackageContent.CreatedAt);
            Assert.Equal(postPackageContent.ModifiedAt, patchPackageContent.ModifiedAt);
            Assert.Equal(postPackageContent.ArrivedAt, postPackageContent.ArrivedAt);
            Assert.Equal(postPackageContent.SentAt, postPackageContent.SentAt);
            Assert.Equal(postPackageContent.ReceivedAt, postPackageContent.ReceivedAt);

            Assert.Equal((int)PackageStatusesEnum.Archived, patchPackageContent.StatusId);
            Assert.NotNull(patchPackageContent.ArchivedAt);
            // archive package ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderUserToRemove = db.Users.Single(u => u.Id == postSenderUserContent.Id);
                db.Users.Remove(senderUserToRemove);
                var receiverUserToRemove = db.Users.Single(u => u.Id == postReceiverUserContent.Id);
                db.Users.Remove(receiverUserToRemove);
                var packageToRemove = db.Packages.Single(p => p.Id == postPackageContent.Id);
                db.Packages.Remove(packageToRemove);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PATCH_ClientArchivesPackage_ReturnsForbidden()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);
            var patchPackageMessage = new HttpRequestMessage();

            patchPackageMessage.Method = HttpMethod.Patch;
            patchPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            patchPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{Guid.NewGuid()}/archive"
            );

            var patchPackageResponse = await _httpClient.SendAsync(patchPackageMessage);
            Assert.Equal(HttpStatusCode.Forbidden, patchPackageResponse.StatusCode);
        }

        [Fact]
        public async Task PATCH_ArchivesNonExistentPackage_ReturnsNotFound()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);
            var patchPackageMessage = new HttpRequestMessage();

            patchPackageMessage.Method = HttpMethod.Patch;
            patchPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            patchPackageMessage.RequestUri = new Uri(
                $"http://localhost:8000/packages/{Guid.NewGuid()}/archive"
            );

            var patchPackageResponse = await _httpClient.SendAsync(patchPackageMessage);
            Assert.Equal(HttpStatusCode.NotFound, patchPackageResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AnonymousGetsPackages_ReturnsUnauthorized()
        {
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );

            var getPackagesResponse = await _httpClient.GetAsync(url);
            Assert.Equal(HttpStatusCode.Unauthorized, getPackagesResponse.StatusCode);
        }

        [Fact]
        public async Task GET_GetsPackagesWithInvalidParams_ReturnsBadRequest()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 0, Limit = 0 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getPackagesMessage = new HttpRequestMessage();
            getPackagesMessage.Method = HttpMethod.Get;
            getPackagesMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackagesMessage.RequestUri = new Uri(url);
            var getPackagesResponse = await _httpClient.SendAsync(getPackagesMessage);
            Assert.Equal(HttpStatusCode.BadRequest, getPackagesResponse.StatusCode);
        }

        [Fact]
        public async Task GET_GetsEmptyPackagesList_ReturnsOk()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getPackagesMessage = new HttpRequestMessage();
            getPackagesMessage.Method = HttpMethod.Get;
            getPackagesMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackagesMessage.RequestUri = new Uri(url);
            var getPackagesResponse = await _httpClient.SendAsync(getPackagesMessage);
            Assert.Equal(HttpStatusCode.OK, getPackagesResponse.StatusCode);

            var getPackagesContent = await getPackagesResponse.Content.ReadFromJsonAsync<
                PaginationResponse<PackageResponse>
            >();
            Assert.NotNull(getPackagesContent);
            Assert.Equal(0, getPackagesContent.TotalCount);
            Assert.Equal(0, getPackagesContent.TotalPages);
            Assert.Empty(getPackagesContent.List);
        }

        [Fact]
        public async Task GET_AdminGetsPackagesList_ReturnsOkAllPackages()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Admin);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postSenderUserMessage.Content = JsonContent.Create(senderUser);

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Money,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100m,
                DeliveryPrice = 10m,
                Description = "Test package",
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");
            postPackageMessage.Content = JsonContent.Create(package);

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // get packages ↓
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getPackagesMessage = new HttpRequestMessage();
            getPackagesMessage.Method = HttpMethod.Get;
            getPackagesMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackagesMessage.RequestUri = new Uri(url);
            var getPackagesResponse = await _httpClient.SendAsync(getPackagesMessage);
            Assert.Equal(HttpStatusCode.OK, getPackagesResponse.StatusCode);
            var getPackagesContent = await getPackagesResponse.Content.ReadFromJsonAsync<
                PaginationResponse<PackageResponse>
            >();
            Assert.NotNull(getPackagesContent);
            Assert.Equal(1, getPackagesContent.TotalCount);
            Assert.Equal(1, getPackagesContent.TotalPages);
            Assert.Single(getPackagesContent.List);
            // get packages ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderToRemove = dbContext.Users.Single(u => u.Id == postSenderUserContent.Id);
                dbContext.Users.Remove(senderToRemove);
                var receiverToRemove = dbContext.Users.Single(
                    u => u.Id == postReceiverUserContent.Id
                );
                dbContext.Users.Remove(receiverToRemove);
                var packageToRemove = dbContext.Packages.Single(p => p.Id == postPackageContent.Id);
                dbContext.Packages.Remove(packageToRemove);
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_ManagerGetsPackagesList_ReturnsOkAllPackages()
        {
            var login = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postSenderUserMessage.Content = JsonContent.Create(senderUser);

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Money,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100m,
                DeliveryPrice = 10m,
                Description = "Test package",
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");
            postPackageMessage.Content = JsonContent.Create(package);

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create package ↑

            // get packages ↓
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getPackagesMessage = new HttpRequestMessage();
            getPackagesMessage.Method = HttpMethod.Get;
            getPackagesMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getPackagesMessage.RequestUri = new Uri(url);
            var getPackagesResponse = await _httpClient.SendAsync(getPackagesMessage);
            Assert.Equal(HttpStatusCode.OK, getPackagesResponse.StatusCode);
            var getPackagesContent = await getPackagesResponse.Content.ReadFromJsonAsync<
                PaginationResponse<PackageResponse>
            >();
            Assert.NotNull(getPackagesContent);
            Assert.Equal(1, getPackagesContent.TotalCount);
            Assert.Equal(1, getPackagesContent.TotalPages);
            Assert.Single(getPackagesContent.List);
            // get packages ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderToRemove = dbContext.Users.Single(u => u.Id == postSenderUserContent.Id);
                dbContext.Users.Remove(senderToRemove);
                var receiverToRemove = dbContext.Users.Single(
                    u => u.Id == postReceiverUserContent.Id
                );
                dbContext.Users.Remove(receiverToRemove);
                var packageToRemove = dbContext.Packages.Single(p => p.Id == postPackageContent.Id);
                dbContext.Packages.Remove(packageToRemove);
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_ClientGetsClientUserIdPackagesList_IfClientIsReceiver_ReturnsOk()
        {
            var managerLogin = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var clientLogin = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);
            var existingReceiverUser = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postSenderUserMessage.Content = JsonContent.Create(senderUser);

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create client-related package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Money,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = existingReceiverUser.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100m,
                DeliveryPrice = 10m,
                Description = "Test package",
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");
            postPackageMessage.Content = JsonContent.Create(package);

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create client-related package ↑

            // create filler package NOT related to other user ↓
            var fillerPackage = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Money,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100m,
                DeliveryPrice = 10m,
                Description = "Test package",
            };
            var postFillerPackageMessage = new HttpRequestMessage();

            postFillerPackageMessage.Method = HttpMethod.Post;
            postFillerPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postFillerPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");
            postFillerPackageMessage.Content = JsonContent.Create(fillerPackage);

            var postFillerPackageResponse = await _httpClient.SendAsync(postFillerPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postFillerPackageResponse.StatusCode);

            var postFillerPackageContent =
                await postFillerPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postFillerPackageContent);
            // create filler package NOT related to other user ↑


            // get packages ↓
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getPackagesMessage = new HttpRequestMessage();
            getPackagesMessage.Method = HttpMethod.Get;
            getPackagesMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            getPackagesMessage.RequestUri = new Uri(url);
            var getPackagesResponse = await _httpClient.SendAsync(getPackagesMessage);
            Assert.Equal(HttpStatusCode.OK, getPackagesResponse.StatusCode);
            var getPackagesContent = await getPackagesResponse.Content.ReadFromJsonAsync<
                PaginationResponse<PackageResponse>
            >();
            Assert.NotNull(getPackagesContent);
            Assert.Equal(1, getPackagesContent.TotalCount);
            Assert.Equal(1, getPackagesContent.TotalPages);
            Assert.Single(getPackagesContent.List);
            Assert.True(
                getPackagesContent.List.All(p => p.ReceiverUser.Id == existingReceiverUser.Id)
            );
            // get packages ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var senderToRemove = dbContext.Users.Single(u => u.Id == postSenderUserContent.Id);
                var receiverToRemove = dbContext.Users.Single(
                    u => u.Id == postReceiverUserContent.Id
                );
                var userPackageToRemove = dbContext.Packages.Single(
                    p => p.Id == postPackageContent.Id
                );
                var fillerPackageToRemove = dbContext.Packages.Single(
                    p => p.Id == postFillerPackageContent.Id
                );

                dbContext.Users.Remove(senderToRemove);
                dbContext.Users.Remove(receiverToRemove);
                dbContext.Packages.Remove(userPackageToRemove);
                dbContext.Packages.Remove(fillerPackageToRemove);
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GET_ClientGetsClientUserIdPackagesList_IfClientIsSender_ReturnsOk()
        {
            var managerLogin = await _httpClient.LoginViaEmailAs(UserRolesEnum.Manager);
            var clientLogin = await _httpClient.LoginViaEmailAs(UserRolesEnum.Client);
            var existingSenderUser = UsersHelper.GetExistingUserModel(UserRolesEnum.Client);

            // create receiver user ↓
            var receiverUser = UsersHelper.GetUserRequest(TestUsersEnum.ReceiverClient);
            var postReceiverUserMessage = new HttpRequestMessage();

            postReceiverUserMessage.Method = HttpMethod.Post;
            postReceiverUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postReceiverUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postReceiverUserMessage.Content = JsonContent.Create(receiverUser);

            var postReceiverUserResponse = await _httpClient.SendAsync(postReceiverUserMessage);
            Assert.Equal(HttpStatusCode.OK, postReceiverUserResponse.StatusCode);

            var postReceiverUserContent =
                await postReceiverUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postReceiverUserContent);
            // create receiver user ↑

            // create sender user ↓
            var senderUser = UsersHelper.GetUserRequest(TestUsersEnum.SenderClient);
            var postSenderUserMessage = new HttpRequestMessage();

            postSenderUserMessage.Method = HttpMethod.Post;
            postSenderUserMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postSenderUserMessage.RequestUri = new Uri("http://localhost:8000/users");
            postSenderUserMessage.Content = JsonContent.Create(senderUser);

            var postSenderUserResponse = await _httpClient.SendAsync(postSenderUserMessage);
            Assert.Equal(HttpStatusCode.OK, postSenderUserResponse.StatusCode);

            var postSenderUserContent =
                await postSenderUserResponse.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(postSenderUserContent);
            // create sender user ↑

            var senderDepartment = DepartmentsHelper.GetDepartmentModel(1);
            var receiverDepartment = DepartmentsHelper.GetDepartmentModel(2);

            // create client-related package ↓
            var package = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Money,
                SenderUserId = existingSenderUser.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100m,
                DeliveryPrice = 10m,
                Description = "Test package",
            };
            var postPackageMessage = new HttpRequestMessage();

            postPackageMessage.Method = HttpMethod.Post;
            postPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");
            postPackageMessage.Content = JsonContent.Create(package);

            var postPackageResponse = await _httpClient.SendAsync(postPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postPackageResponse.StatusCode);

            var postPackageContent =
                await postPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postPackageContent);
            // create client-related package ↑

            // create filler package NOT related to client ↓
            var fillerPackage = new CreatePackageRequest
            {
                CategoryId = (int)PackageCategoriesEnum.Money,
                SenderUserId = postReceiverUserContent.Id,
                ReceiverUserId = postSenderUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100m,
                DeliveryPrice = 10m,
                Description = "Test package",
            };
            var postFillerPackageMessage = new HttpRequestMessage();

            postFillerPackageMessage.Method = HttpMethod.Post;
            postFillerPackageMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            postFillerPackageMessage.RequestUri = new Uri("http://localhost:8000/packages");
            postFillerPackageMessage.Content = JsonContent.Create(fillerPackage);

            var postFillerPackageResponse = await _httpClient.SendAsync(postFillerPackageMessage);
            Assert.Equal(HttpStatusCode.Created, postFillerPackageResponse.StatusCode);

            var postFillerPackageContent =
                await postFillerPackageResponse.Content.ReadFromJsonAsync<PackageResponse>();
            Assert.NotNull(postFillerPackageContent);
            // create filler package NOT related to client ↑

            // get packages ↓
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/packages",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var getPackagesMessage = new HttpRequestMessage();
            getPackagesMessage.Method = HttpMethod.Get;
            getPackagesMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            getPackagesMessage.RequestUri = new Uri(url);
            var getPackagesResponse = await _httpClient.SendAsync(getPackagesMessage);
            Assert.Equal(HttpStatusCode.OK, getPackagesResponse.StatusCode);
            var getPackagesContent = await getPackagesResponse.Content.ReadFromJsonAsync<
                PaginationResponse<PackageResponse>
            >();
            Assert.NotNull(getPackagesContent);
            Assert.Equal(1, getPackagesContent.TotalCount);
            Assert.Equal(1, getPackagesContent.TotalPages);
            Assert.Single(getPackagesContent.List);
            Assert.True(getPackagesContent.List.All(p => p.SenderUser.Id == existingSenderUser.Id));
            // get packages ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var receiverToRemove = dbContext.Users.Single(
                    u => u.Id == postReceiverUserContent.Id
                );
                var senderToRemove = dbContext.Users.Single(u => u.Id == postSenderUserContent.Id);
                var userPackageToRemove = dbContext.Packages.Single(
                    p => p.Id == postPackageContent.Id
                );
                var fillerPackageToRemove = dbContext.Packages.Single(
                    p => p.Id == postFillerPackageContent.Id
                );

                dbContext.Users.Remove(receiverToRemove);
                dbContext.Users.Remove(senderToRemove);
                dbContext.Packages.Remove(userPackageToRemove);
                dbContext.Packages.Remove(fillerPackageToRemove);
                dbContext.SaveChanges();
            }
        }
    }
}

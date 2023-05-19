﻿using HyperPost.DB;
using HyperPost.DTO.Package;
using HyperPost.DTO.User;
using HyperPost.Migrations;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            Assert.Equal((int)StatusesEnum.Created, postPackageContent.StatusId);
            Assert.Equal(package.SenderUserId, postPackageContent.SenderUserId);
            Assert.Equal(package.ReceiverUserId, postPackageContent.ReceiverUserId);
            Assert.Equal(package.SenderDepartmentId, postPackageContent.SenderDepartmentId);
            Assert.Equal(package.ReceiverDepartmentId, postPackageContent.ReceiverDepartmentId);
            Assert.Equal(package.Weight, postPackageContent.Weight);
            Assert.Equal(package.PackagePrice, postPackageContent.PackagePrice);
            Assert.Equal(package.DeliveryPrice, postPackageContent.DeliveryPrice);

            Assert.Null(postPackageContent.ModifiedAt);
            Assert.Null(postPackageContent.SentAt);
            Assert.Null(postPackageContent.ArrivedAt);
            Assert.Null(postPackageContent.ReceivedAt);
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
            // create and ger sender ↑

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
            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            Assert.Equal((int)StatusesEnum.Created, postPackageContent.StatusId);
            Assert.Equal(package.SenderUserId, postPackageContent.SenderUserId);
            Assert.Equal(package.ReceiverUserId, postPackageContent.ReceiverUserId);
            Assert.Equal(package.SenderDepartmentId, postPackageContent.SenderDepartmentId);
            Assert.Equal(package.ReceiverDepartmentId, postPackageContent.ReceiverDepartmentId);
            Assert.Equal(package.Weight, postPackageContent.Weight);
            Assert.Equal(package.PackagePrice, postPackageContent.PackagePrice);
            Assert.Equal(package.DeliveryPrice, postPackageContent.DeliveryPrice);
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = 55,
                ReceiverUserId = 56,
                SenderDepartmentId = 57,
                ReceiverDepartmentId = 58,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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
            var package = new PackageRequest();

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

            var package = new PackageRequest
            {
                CategoryId = 0,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartmend.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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
            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = 0,
                ReceiverUserId = 56,
                SenderDepartmentId = 57,
                ReceiverDepartmentId = 58,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = 0,
                SenderDepartmentId = 57,
                ReceiverDepartmentId = 58,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postSenderUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = senderDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = 0,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = 0,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 100000000.5m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.555m,
                DeliveryPrice = 5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 100000000.5m,
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
            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.555m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 4.99m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 100.5m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 1.555m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
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

            var package = new PackageRequest
            {
                CategoryId = (int)CategoriesEnum.Books,
                SenderUserId = postSenderUserContent.Id,
                ReceiverUserId = postReceiverUserContent.Id,
                SenderDepartmentId = senderDepartment.Id,
                ReceiverDepartmentId = receiverDepartment.Id,
                Weight = 0.1m,
                PackagePrice = 10.5m,
                DeliveryPrice = 5.5m,
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
    }
}

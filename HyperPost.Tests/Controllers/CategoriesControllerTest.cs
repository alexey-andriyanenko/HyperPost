using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HyperPost.DB;
using HyperPost.DTO.Category;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace HyperPost.Tests.Controllers
{
    public class CategoriesControllerTest : IClassFixture<HyperPostTestFactory<Program>>
    {
        private readonly HyperPostTestFactory<Program> _factory;
        private readonly HttpClient _client;

        public CategoriesControllerTest(HyperPostTestFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_AdminCreatesCategory_ReturnsCreated()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var category = new PackageCategoryRequest { Name = "Test Category", };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(category);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/package/categories");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.Equal(category.Name, content.Name);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(u => u.Name == "Test Category");
                db.PackageCategoties.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_ManagerCreatedCategory_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var category = new PackageCategoryRequest { Name = "Test Category", };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(category);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/package/categories");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_ClientCreatesCategory_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var category = new PackageCategoryRequest { Name = "Test Category", };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(category);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/package/categories");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_AdminCreatesPackageCategoryWithNameMoreThan30Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var category = new PackageCategoryRequest
            {
                Name = "Test Category With Name More Than 30 Characters",
            };
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(category);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/package/categories");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_AdminCreatesPackageCategoryWithExistingName_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var category = new PackageCategoryRequest { Name = "Repeated Name", };

            // create first category ↓
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(category);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/package/categories");
            // create first category ↑

            // create second category with same name ↓
            var firstResponse = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(category);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/package/categories");

            var secondResponse = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
            // create second category with same name ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(u => u.Name == "Repeated Name");
                db.PackageCategoties.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_AdminUpdatesCategegory_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create category ↓
            var postCategory = new PackageCategoryRequest { Name = "Test Category", };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postCategory);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/package/categories");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent =
                await postResponse.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(postContent);
            Assert.Equal(postCategory.Name, postContent.Name);
            // create category ↑

            // update category ↓
            var putCategory = new PackageCategoryRequest { Name = "Updated Test Category", };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putCategory);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{postContent.Id}"
            );

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(putCategory.Name, putContent.Name);
            // update category ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(c => c.Id == postContent.Id);
                db.PackageCategoties.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ManagerUpdatesCategory_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create category ↓
            var postCategory = new PackageCategoryRequest { Name = "Test Category", };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postCategory);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/package/categories");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent =
                await postResponse.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(postContent);
            Assert.Equal(postCategory.Name, postContent.Name);
            // create category ↑

            // update category ↓
            var putCategory = new PackageCategoryRequest { Name = "Updated Test Category", };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putCategory);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            putMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{postContent.Id}"
            );

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update category ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(c => c.Id == postContent.Id);
                db.PackageCategoties.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ClientUpdatesCategory_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);

            // create category ↓
            var postCategory = new PackageCategoryRequest { Name = "Test Category", };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postCategory);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/package/categories");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent =
                await postResponse.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(postContent);
            Assert.Equal(postCategory.Name, postContent.Name);
            // create category ↑

            // update category ↓
            var putCategory = new PackageCategoryRequest { Name = "Updated Test Category", };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putCategory);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            putMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{postContent.Id}"
            );

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update category ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(c => c.Id == postContent.Id);
                db.PackageCategoties.Remove(model);
                db.SaveChanges();
            }
        }
    }
}

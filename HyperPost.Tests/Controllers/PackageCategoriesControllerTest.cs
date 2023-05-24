using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HyperPost.DB;
using HyperPost.DTO.PackageCategory;
using HyperPost.DTO.Pagination;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace HyperPost.Tests.Controllers
{
    public class PackageCategoriesControllerTest : IClassFixture<HyperPostTestFactory<Program>>
    {
        private readonly HyperPostTestFactory<Program> _factory;
        private readonly HttpClient _client;

        public PackageCategoriesControllerTest(HyperPostTestFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_AdminCreatesCategory_ReturnsCreated()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var category = new CreatePackageCategoryRequest { Name = "Test Category", };

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
            var category = new CreatePackageCategoryRequest { Name = "Test Category", };

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
            var category = new CreatePackageCategoryRequest { Name = "Test Category", };

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
            var category = new CreatePackageCategoryRequest
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
            var category = new CreatePackageCategoryRequest { Name = "Repeated Name", };

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
            var postCategory = new CreatePackageCategoryRequest { Name = "Test Category", };
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
            var putCategory = new UpdatePackageCategoryRequest { Name = "Updated Test Category", };
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
            var postCategory = new CreatePackageCategoryRequest { Name = "Test Category", };
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
            var putCategory = new UpdatePackageCategoryRequest { Name = "Updated Test Category", };
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
            var postCategory = new UpdatePackageCategoryRequest { Name = "Test Category", };
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
            var putCategory = new UpdatePackageCategoryRequest { Name = "Updated Test Category", };
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

        [Fact]
        public async Task PUT_UpdatesCategoryWithSameName_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            // create category ↓
            var postCategory = new CreatePackageCategoryRequest { Name = "Test Category", };
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
            var putCategory = new CreatePackageCategoryRequest { Name = postCategory.Name, };
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
        public async Task PUT_UpdatesCategoryWithExistingName_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create category ↓
            var postCategory = new CreatePackageCategoryRequest { Name = "Test Category", };
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

            // create category ↓
            var postCategory2 = new CreatePackageCategoryRequest { Name = "Test Category 2", };
            var postMessage2 = new HttpRequestMessage();

            postMessage2.Method = HttpMethod.Post;
            postMessage2.Content = JsonContent.Create(postCategory2);
            postMessage2.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage2.RequestUri = new Uri("http://localhost:8000/package/categories");
            var postResponse2 = await _client.SendAsync(postMessage2);
            Assert.Equal(HttpStatusCode.Created, postResponse2.StatusCode);

            var postContent2 =
                await postResponse2.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(postContent2);
            Assert.Equal(postCategory2.Name, postContent2.Name);
            // create category ↑

            // update category ↓
            var putCategory = new UpdatePackageCategoryRequest { Name = postCategory2.Name, };
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
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update category ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(c => c.Id == postContent.Id);
                db.PackageCategoties.Remove(model);
                var model2 = db.PackageCategoties.Single(c => c.Id == postContent2.Id);
                db.PackageCategoties.Remove(model2);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesCategoryWithNameMoreThan30Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create category ↓
            var postCategory = new CreatePackageCategoryRequest { Name = "Test Category", };
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
            var putCategory = new UpdatePackageCategoryRequest
            {
                Name = "Test Category With Name More Than 30 Characters",
            };
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
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
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
        public async Task PUT_UpdatesCategory_ReturnsNotFound()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var putCategory = new CreatePackageCategoryRequest { Name = "Updated Test Category", };

            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putCategory);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{PackageCategoriesEnum.NonExistent}"
            );

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsCategory_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var existingCategory = PackageCategoriesHelper.GetPackageCategoryModel(
                PackageCategoriesEnum.Books
            );

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{existingCategory.Id}"
            );

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(content);
            Assert.Equal(existingCategory.Id, content.Id);
            Assert.Equal(existingCategory.Name, content.Name);
        }

        [Fact]
        public async Task GET_ManagerGetsCategory_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var existingCategory = PackageCategoriesHelper.GetPackageCategoryModel(
                PackageCategoriesEnum.Books
            );

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{existingCategory.Id}"
            );

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(content);
            Assert.Equal(existingCategory.Id, content.Id);
            Assert.Equal(existingCategory.Name, content.Name);
        }

        [Fact]
        public async Task GET_ClientGetsCategory_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var existingCategory = PackageCategoriesHelper.GetPackageCategoryModel(
                PackageCategoriesEnum.Books
            );

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{existingCategory.Id}"
            );
            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<PackageCategoryResponse>();
            Assert.NotNull(content);
            Assert.Equal(existingCategory.Id, content.Id);
            Assert.Equal(existingCategory.Name, content.Name);
        }

        [Fact]
        public async Task GET_GetsCategory_ReturnsNotFound()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{PackageCategoriesEnum.NonExistent}"
            );

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_AdminDeletesCategory_ReturnsNoContent()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var category = new CreatePackageCategoryRequest { Name = "Category to delete" };

            // create category ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(category);
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
            // create category ↑

            // delete category ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            deleteMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{postContent.Id}"
            );

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            // delete category ↑

            // get category ↓
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/${postContent.Id}"
            );

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            // get category ↑
        }

        [Fact]
        public async Task DELETE_Manager_DeletesCategory_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var managerLogin = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var category = new CreatePackageCategoryRequest { Name = "Category to delete" };

            // create category ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(category);
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
            // create category ↑

            // delete category ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                managerLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{postContent.Id}"
            );

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            // delete category ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(x => x.Id == postContent.Id);
                db.PackageCategoties.Remove(model);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task DELETE_ClientDeletesCategory_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var category = new CreatePackageCategoryRequest { Name = "Category to delete" };

            // create category ↓
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(category);
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
            // create category ↑

            // delete category ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri(
                $"http://localhost:8000/package/categories/{postContent.Id}"
            );

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            // delete category ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.PackageCategoties.Single(x => x.Id == postContent.Id);
                db.PackageCategoties.Remove(model);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task GET_AnonymousGetsCategories_ReturnsUnauthorized()
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://localhost:8000/package/categories");
            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_GetsCategoriesWithInvalidParams_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 0, Limit = 0 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/package/categories",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(url);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GET_GetsEmptyCategoriesList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 3, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/package/categories",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(url);
            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<
                PaginationResponse<PackageCategoryResponse>
            >();
            Assert.NotNull(content);
            Assert.Equal(11, content.TotalCount);
            Assert.Equal(2, content.TotalPages);
            Assert.Empty(content.List);
        }

        [Fact]
        public async Task Admin_GetsCategoriesList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/package/categories",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(url);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<
                PaginationResponse<PackageCategoryResponse>
            >();
            Assert.NotNull(content);
            Assert.Equal(11, content.TotalCount);
            Assert.Equal(2, content.TotalPages);
            Assert.Equal(10, content.List.Count);
        }

        [Fact]
        public async Task Manager_GetsCategoriesList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/package/categories",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(url);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<
                PaginationResponse<PackageCategoryResponse>
            >();

            Assert.NotNull(content);
            Assert.Equal(11, content.TotalCount);
            Assert.Equal(2, content.TotalPages);
            Assert.Equal(10, content.List.Count);
        }

        [Fact]
        public async Task Client_GetsCategoriesList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/package/categories",
                new Dictionary<string, string?>
                {
                    { "page", paginationRequest.Page.ToString() },
                    { "limit", paginationRequest.Limit.ToString() }
                }
            );
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri(url);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<
                PaginationResponse<PackageCategoryResponse>
            >();
            Assert.NotNull(content);
            Assert.Equal(11, content.TotalCount);
            Assert.Equal(2, content.TotalPages);
            Assert.Equal(10, content.List.Count);
        }
    }
}

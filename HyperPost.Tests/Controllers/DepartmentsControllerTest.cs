using HyperPost.DTO.Department;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using HyperPost.DB;

namespace HyperPost.Tests.Controllers
{
    public class DepartmentsControllerTest : IClassFixture<HyperPostTestFactory<Program>>
    {
        private readonly HyperPostTestFactory<Program> _factory;
        private readonly HttpClient _client;

        public DepartmentsControllerTest(HyperPostTestFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_AnonymousCreatesDepartment_ReturnsUnauthorized()
        {
            var department = new DepartmentRequest { Number = 99, FullAddress = "Test Address", };
            var response = await _client.PostAsJsonAsync(
                "http://localhost:8000/departments",
                department
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task POST_AdminCreatesDepartment_ReturnsCreated()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var department = new DepartmentRequest { Number = 99, FullAddress = "Test Address", };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(department);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.Equal(department.Number, content.Number);
            Assert.Equal(department.FullAddress, content.FullAddress);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Departments.Single(u => u.Number == 99);

                db.Departments.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_ManagerCreatesDepartment_ReturnsCreated()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var department = new DepartmentRequest { Number = 99, FullAddress = "Test Address", };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(department);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.Equal(department.Number, content.Number);
            Assert.Equal(department.FullAddress, content.FullAddress);

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var model = db.Departments.Single(u => u.Number == 99);
                db.Departments.Remove(model);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task POST_ManagerCreatesDepartmentWithExistingNumber_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var department = new DepartmentRequest { Number = 1, FullAddress = "Test Address", };
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(department);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_ManagerCreatesDepartmentWithFullAddressMoreThan100Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var department = new DepartmentRequest
            {
                Number = 98,
                FullAddress =
                    "lllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll"
            };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(department);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_ManagerCreatesDepartmentWithInvalidDepartmentRequest_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var department = new DepartmentRequest { Number = 98, FullAddress = null };

            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Post;
            message.Content = JsonContent.Create(department);
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GET_AnonymousGetsDepartment_ReturnsUnauthorized()
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://localhost:8000/departments/1");
            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsDepartment_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var message = new HttpRequestMessage();
            var department = DepartmentsHelper.GetDepartmentModel(1);

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/departments/{department.Id}");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(content);
            Assert.Equal(department.Id, content.Id);
            Assert.Equal(department.Number, content.Number);
            Assert.Equal(department.FullAddress, content.FullAddress);
        }

        [Fact]
        public async Task GET_ManagerGetsDepartment_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var message = new HttpRequestMessage();
            var department = DepartmentsHelper.GetDepartmentModel(1);

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/departments/{department.Id}");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(content);
            Assert.Equal(department.Id, content.Id);
            Assert.Equal(department.Number, content.Number);
            Assert.Equal(department.FullAddress, content.FullAddress);
        }

        [Fact]
        public async Task GET_ClientGetsDepartment_ReturnsOK()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var message = new HttpRequestMessage();
            var department = DepartmentsHelper.GetDepartmentModel(1);

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/departments/{department.Id}");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(content);
            Assert.Equal(department.Id, content.Id);
            Assert.Equal(department.Number, content.Number);
            Assert.Equal(department.FullAddress, content.FullAddress);
        }

        [Fact]
        public async Task GET_GetsNonExistentDepartment_ReturnsNotFound()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            message.RequestUri = new Uri($"http://localhost:8000/departments/0");

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

using HyperPost.DTO.Department;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using HyperPost.DB;
using HyperPost.DTO.Pagination;
using Microsoft.AspNetCore.WebUtilities;

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
            var department = new CreateDepartmentRequest
            {
                Number = 99,
                FullAddress = "Test Address",
            };
            var response = await _client.PostAsJsonAsync(
                "http://localhost:8000/departments",
                department
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task POST_ClientCreatesDepartment_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var department = new CreateDepartmentRequest
            {
                Number = 99,
                FullAddress = "Test Address",
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
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_AdminCreatesDepartment_ReturnsCreated()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var department = new CreateDepartmentRequest
            {
                Number = 99,
                FullAddress = "Test Address",
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
            var department = new CreateDepartmentRequest
            {
                Number = 99,
                FullAddress = "Test Address",
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
            var department = new CreateDepartmentRequest
            {
                Number = 1,
                FullAddress = "Test Address",
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
        public async Task POST_ManagerCreatesDepartmentWithFullAddressMoreThan100Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var department = new CreateDepartmentRequest
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
            var department = new CreateDepartmentRequest { Number = 98, FullAddress = null };

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

        [Fact]
        public async Task PUT_AnonymousUpdatesDepartment_ReturnsUnauthorized()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 98,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(content);
            // create department ↑

            // update department ↓
            var putDepartment = new UpdateDepartmentRequest { FullAddress = "updated address" };
            var putResponse = await _client.PutAsJsonAsync(
                $"http://localhost:8000/departments/{content.Id}",
                putDepartment
            );
            Assert.Equal(HttpStatusCode.Unauthorized, putResponse.StatusCode);
            // update department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == content.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ClientUpdatesDepartment_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 98,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var response = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<DepartmentResponse>();

            Assert.NotNull(content);
            // create department ↑

            // update department ↓
            var putDepartment = new UpdateDepartmentRequest { FullAddress = "updated address" };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putDepartment);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/departments/{content.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
            // update department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == content.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_AdminUpdatesDepartment_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 97,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // update department ↓
            var putDepartment = new UpdateDepartmentRequest { FullAddress = "updated address" };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putDepartment);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/departments/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(postContent.Id, putContent.Id);
            Assert.Equal(putDepartment.FullAddress, putContent.FullAddress);
            // update department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_ManagerUpdatesDepartment_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 96,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // update department ↓
            var putDepartment = new UpdateDepartmentRequest { FullAddress = "updated address" };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putDepartment);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/departments/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var putContent = await putResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(putContent);
            Assert.Equal(postContent.Id, putContent.Id);
            Assert.Equal(putDepartment.FullAddress, putContent.FullAddress);
            // update department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesNonExistingDepartment_ReturnsNotFound()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var putDepartment = new UpdateDepartmentRequest { FullAddress = "updated address" };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putDepartment);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/departments/0");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
        }

        [Fact]
        public async Task PUT_UpdatesDepartmentWithInvalidData_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 95,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // update department ↓
            var putDepartment = new UpdateDepartmentRequest { FullAddress = "" };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putDepartment);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/departments/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task PUT_UpdatesDepartmentWithFullAdressMoreThan100Characters_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 95,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // update department ↓
            var putDepartment = new UpdateDepartmentRequest { FullAddress = new string('a', 101) };
            var putMessage = new HttpRequestMessage();

            putMessage.Method = HttpMethod.Put;
            putMessage.Content = JsonContent.Create(putDepartment);
            putMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            putMessage.RequestUri = new Uri($"http://localhost:8000/departments/{postContent.Id}");

            var putResponse = await _client.SendAsync(putMessage);
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
            // update department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_AnonymousDeletesDepartment_ReturnsUnauthorized()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 95,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // delete department ↓
            var deleteResponse = await _client.DeleteAsync(
                $"http://localhost:8000/departments/{postContent.Id}"
            );
            Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
            // delete department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_AdminDeletesDepartment_ReturnsNoContent()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 95,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // delete department ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            deleteMessage.RequestUri = new Uri(
                $"http://localhost:8000/departments/{postContent.Id}"
            );

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            // delete department ↑

            // get department ↓
            var getMessage = new HttpRequestMessage();

            getMessage.Method = HttpMethod.Get;
            getMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            getMessage.RequestUri = new Uri($"http://localhost:8000/departments/{postContent.Id}");

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            // get department ↑
        }

        [Fact]
        public async Task DELETE_ManagerDeletesDepartment_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 95,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // delete department ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            deleteMessage.RequestUri = new Uri(
                $"http://localhost:8000/departments/{postContent.Id}"
            );

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            // delete department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_ClientDeletesDepartment_ReturnsForbidden()
        {
            var adminLogin = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var clientLogin = await _client.LoginViaEmailAs(UserRolesEnum.Client);

            // create department ↓
            var postDepartment = new CreateDepartmentRequest
            {
                Number = 95,
                FullAddress = "address"
            };
            var postMessage = new HttpRequestMessage();

            postMessage.Method = HttpMethod.Post;
            postMessage.Content = JsonContent.Create(postDepartment);
            postMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                adminLogin.AccessToken
            );
            postMessage.RequestUri = new Uri("http://localhost:8000/departments");

            var postResponse = await _client.SendAsync(postMessage);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var postContent = await postResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
            Assert.NotNull(postContent);
            // create department ↑

            // delete department ↓
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                clientLogin.AccessToken
            );
            deleteMessage.RequestUri = new Uri(
                $"http://localhost:8000/departments/{postContent.Id}"
            );

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            // delete department ↑

            // cleanup ↓
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HyperPostDbContext>();
                var departmentToDelete = context.Departments.Single(d => d.Id == postContent.Id);
                context.Departments.Remove(departmentToDelete);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task DELETE_DeletesNonExistentDepartment_ReturnsNotFound()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var deleteMessage = new HttpRequestMessage();

            deleteMessage.Method = HttpMethod.Delete;
            deleteMessage.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                login.AccessToken
            );
            deleteMessage.RequestUri = new Uri("http://localhost:8000/departments/0");

            var deleteResponse = await _client.SendAsync(deleteMessage);
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GET_AnonymousGetsDepartments_ReturnsUnauthorized()
        {
            var getMessage = new HttpRequestMessage();
            getMessage.Method = HttpMethod.Get;
            getMessage.RequestUri = new Uri("http://localhost:8000/departments");
            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }

        [Fact]
        public async Task GET_GetsDepartmentWithInvalidParams_ReturnsBadRequest()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 0, Limit = 0 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/departments",
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
                login.AccessToken
            );
            getMessage.RequestUri = new Uri(url);

            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }

        [Fact]
        public async Task GET_GetsEmptyDepartmentsList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 3, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/departments",
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
                login.AccessToken
            );
            getMessage.RequestUri = new Uri(url);
            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getContent = await getResponse.Content.ReadFromJsonAsync<
                PaginationResponse<DepartmentResponse>
            >();

            Assert.NotNull(getContent);
            Assert.Equal(10, getContent.TotalCount);
            Assert.Equal(1, getContent.TotalPages);
            Assert.Empty(getContent.List);
        }

        [Fact]
        public async Task GET_AdminGetsDepartmentsList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/departments",
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
                login.AccessToken
            );
            getMessage.RequestUri = new Uri(url);
            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getContent = await getResponse.Content.ReadFromJsonAsync<
                PaginationResponse<DepartmentResponse>
            >();
            Assert.NotNull(getContent);
            Assert.Equal(10, getContent.TotalCount);
            Assert.Equal(1, getContent.TotalPages);
            Assert.Equal(10, getContent.List.Count);
        }

        [Fact]
        public async Task GET_ManagerGetsDepartmentsList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/departments",
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
                login.AccessToken
            );
            getMessage.RequestUri = new Uri(url);
            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getContent = await getResponse.Content.ReadFromJsonAsync<
                PaginationResponse<DepartmentResponse>
            >();
            Assert.NotNull(getContent);
            Assert.Equal(10, getContent.TotalCount);
            Assert.Equal(1, getContent.TotalPages);
            Assert.Equal(10, getContent.List.Count);
        }

        [Fact]
        public async Task GET_ClientGetsDepartmentsList_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var paginationRequest = new PaginationRequest { Page = 1, Limit = 10 };
            var url = QueryHelpers.AddQueryString(
                "http://localhost:8000/departments",
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
                login.AccessToken
            );
            getMessage.RequestUri = new Uri(url);
            var getResponse = await _client.SendAsync(getMessage);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getContent = await getResponse.Content.ReadFromJsonAsync<
                PaginationResponse<DepartmentResponse>
            >();
            Assert.NotNull(getContent);
            Assert.Equal(10, getContent.TotalCount);
            Assert.Equal(1, getContent.TotalPages);
            Assert.Equal(10, getContent.List.Count);
        }
    }
}

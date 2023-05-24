using HyperPost.DTO.PackageStatus;
using HyperPost.Shared;
using HyperPost.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HyperPost.Tests.Controllers
{
    public class PackageStatusesControllerTest : IClassFixture<HyperPostTestFactory<Program>>
    {
        private readonly HyperPostTestFactory<Program> _factory;
        private readonly HttpClient _client;

        public PackageStatusesControllerTest(HyperPostTestFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GET_Anonymous_GetsStatuses_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/packages/statuses");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GET_AdminGetsStatuses_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Admin);
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );
            message.RequestUri = new Uri("/packages/statuses", UriKind.Relative);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<List<PackageStatusResponse>>();
            Assert.NotNull(content);
            Assert.Equal(6, content.Count);
        }

        [Fact]
        public async Task ManagerGetsStatuses_ReturnsOk()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Manager);
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );
            message.RequestUri = new Uri("/packages/statuses", UriKind.Relative);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<List<PackageStatusResponse>>();
            Assert.NotNull(content);
            Assert.Equal(6, content.Count);
        }

        [Fact]
        public async Task ClientGetsStatuses_ReturnsForbidden()
        {
            var login = await _client.LoginViaEmailAs(UserRolesEnum.Client);
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );
            message.RequestUri = new Uri("/packages/statuses", UriKind.Relative);

            var response = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}

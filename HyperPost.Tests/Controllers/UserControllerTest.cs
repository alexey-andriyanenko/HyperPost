using MyStore.Tests;

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
        public void Success()
        {
            Assert.Equal(1, 1);
        }
    }
}

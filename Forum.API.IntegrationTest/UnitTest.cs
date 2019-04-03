using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace Forum.API.IntegrationTest
{
    public class UnitTest
    {
        private readonly HttpClient _client;

        public UnitTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }

        [Theory]
        [InlineData("Get")]
        public async Task DiscussionGetAllTest(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "https://localhost:5001/api/discussions");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //[Fact]
        //public void Test1()
        //{

        //}
    }
}

using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
 using System.Net;
using MicroBlog.Auth;
using System.Threading.Tasks;

namespace MicroBlog.IntegrationTests.Tests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly System.Net.Http.HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_Success_WithCorrectCredentials()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new { Username = "admin", Password = "pass123" });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("token");
        }

        [Fact]
        public async Task Login_Fails_WithWrongCredentials()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new { Username = "admin", Password = "wrong" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
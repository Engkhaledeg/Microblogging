using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
 using System.Net;
using MicroBlog.Api;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace MicroBlog.IntegrationTests.Tests
{
    public class PostsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PostsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var login = new { Username = "admin", Password = "pass123" };
            var response = await _client.PostAsJsonAsync("/api/auth/login", login);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("token").GetString();
        }

        [Fact]
        public async Task CreatePost_Unauthorized_WhenNoToken()
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent("Post without token"), "Text" }
            };

            var response = await _client.PostAsync("/api/posts", form);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreatePost_Success_WithTokenAndValidImage()
        {
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var form = new MultipartFormDataContent
            {
                { new StringContent("This is a test post"), "Text" },
                { new StringContent("34.0522"), "Latitude" },
                { new StringContent("-118.2437"), "Longitude" }
            };

            var dummyImagePath = "/mnt/data/test-image.jpg";
            await File.WriteAllBytesAsync(dummyImagePath, new byte[1024]); // 1KB dummy file
            var fileContent = new StreamContent(File.OpenRead(dummyImagePath));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            form.Add(fileContent, "Image", "test-image.jpg");

            var response = await _client.PostAsync("/api/posts", form);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            responseJson.Should().Contain("id");
        }

        [Fact]
        public async Task CreatePost_BadRequest_WhenTextIsTooLong()
        {
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var form = new MultipartFormDataContent
            {
                { new StringContent(new string('a', 150)), "Text" }, // Too long
                { new StringContent("34.0522"), "Latitude" },
                { new StringContent("-118.2437"), "Longitude" }
            };

            var response = await _client.PostAsync("/api/posts", form);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
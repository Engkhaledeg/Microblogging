using Microsoft.AspNetCore.Mvc;
using MicroBlog.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroBlog.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Timeline()
        {
            var token = HttpContext.Session.GetString("jwt");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync("https://localhost:5001/api/posts"); // Adjust port if needed

            if (!response.IsSuccessStatusCode)
            {
                ViewData["Error"] = "Unable to fetch posts.";
                return View(new List<PostDto>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<List<PostDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(posts ?? new List<PostDto>());
        }
    }
}

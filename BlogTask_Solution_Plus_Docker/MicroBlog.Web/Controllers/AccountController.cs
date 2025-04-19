using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace MicroBlog.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public AccountController(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string user, string pass)
        {
            var dto = new { Username = user, Password = pass };
            var client = _httpFactory.CreateClient();
            var baseUrl = _config["ApiBaseUrl"];
            var resp = await client.PostAsync(
                $"{baseUrl}/auth/login",
                new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
            );
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("token").GetString();

            // store in cookie or JS localStorage via JS redirect
            Response.Cookies.Append("microblog_jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            });

            return RedirectToAction("Index", "Home");
        }
    }
}

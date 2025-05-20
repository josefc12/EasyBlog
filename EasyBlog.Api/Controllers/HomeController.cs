using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public async Task<string> GetHomePageContent()
        {
            var filePath = Path.Combine(_env.WebRootPath, "home.html");

            if (!System.IO.File.Exists(filePath))
            {
                return string.Empty;
            }

            var content = await System.IO.File.ReadAllTextAsync(filePath);
            return content;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateHomePageContent(string htmlValue)
        {
            if (string.IsNullOrWhiteSpace(htmlValue))
            {
                return BadRequest("HTML content cannot be empty.");
            }

            var filePath = Path.Combine(_env.WebRootPath, "home.html");

            await System.IO.File.WriteAllTextAsync(filePath, htmlValue);

            return Ok("Home page updated successfully.");
        }
    }
}

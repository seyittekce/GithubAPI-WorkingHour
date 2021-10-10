using Core;
using GithubAPI_WorkingHour.Models;
using GithubAPI_WorkingHour.Models.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using System.Threading.Tasks;

namespace GithubAPI_WorkingHour.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<GithubApiKeys> keys;
        private readonly IGitHubClient client;

        public HomeController(ILogger<HomeController> logger, IGitHubClient client, IOptions<GithubApiKeys> keys)
        {
            _logger = logger;
            this.client = client;
            this.keys = keys;
        }

        public async Task<IActionResult> Index()
        {
            this.client.Connection.Credentials = new Credentials(HttpContext.Session.GetString("OAuthToken"));

            var user = await client.User.Current();
            return View(new IndexModel { User = user });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
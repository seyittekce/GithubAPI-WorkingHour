using Business.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GithubAPI_WorkingHour.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            return View(await _userService.GetUserWorkingHour(startDate, endDate));
        }
    }
}

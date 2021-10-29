using Business.User;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GithubAPI_WorkingHour.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IUserService _userService;
        public RepositoryController(IRepositoryService repositoryService, IUserService userService)
        {
            _repositoryService = repositoryService;
            _userService = userService;
        }
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            return View(await _repositoryService.RepositoryWorkingDaysAsync(startDate, endDate));
        }

        public async Task<IActionResult> GetRepositoryWorkingHourWithUser(string owner, string name, DateTime? startDate, DateTime? endDate)
        {
            return View(await _userService.GetRepositoryHourWithUsers(owner, name, startDate, endDate));
        }
    }
}
using System;
using Core.Abstracts;
using GithubAPI_WorkingHour.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;
using Business.Repository;
using Business.User;


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
        public async  Task<IActionResult> Index(DateTime? startDate,DateTime? endDate)
        {
            return View(await _repositoryService.RepositoryWorkingDaysAsync(startDate,endDate));
        }
       
        public async Task<IActionResult> GetRepositoryWorkingHourWithUser(string owner, string name)
        {
            return View(await _userService.GetRepositoryHourWithUsers(owner, name));
        }
    }
}
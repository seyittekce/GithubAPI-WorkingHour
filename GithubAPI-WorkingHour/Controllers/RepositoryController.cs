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


namespace GithubAPI_WorkingHour.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        public RepositoryController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult IndexDataSource()
        {
            return Json(_repositoryService.RepositoryWorkingDaysAsync(),new JsonSerializerOptions{WriteIndented = true});
        }
    }
}
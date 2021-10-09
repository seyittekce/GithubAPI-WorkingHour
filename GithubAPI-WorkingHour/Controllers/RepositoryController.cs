using Core;
using Core.Abstracts;
using GithubAPI_WorkingHour.Models;
using GithubAPI_WorkingHour.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace GithubAPI_WorkingHour.Controllers
{
    public class RepositoryController : Controller
    {

        private readonly IGitHubClient client;
        private readonly IWorkingHourCalculator workingHourCalculator;

        public RepositoryController(IGitHubClient client, IWorkingHourCalculator workingHourCalculator)
        {

            this.client = client;
            this.workingHourCalculator = workingHourCalculator;
        }
        public async Task<IActionResult> Index()
        {
           
            var token = HttpContext.Session.GetString("OAuthToken");
            this.client.Connection.Credentials = new Credentials(token);
            var repositories = await client.Repository.GetAllForCurrent();
            var repoModel = new List<RepositoryModel>();
            foreach (var item in repositories)
            {
                var issueComments = await client.Issue.Comment.GetAllForRepository(item.Owner.Login, item.Name);
                var total= workingHourCalculator.CalculateTotal(issueComments);
                var day= workingHourCalculator.CalculateDayByDay(issueComments);
                repoModel.Add(new RepositoryModel { Repository=item,TotalWorkingHour=total,RepositoryWorkingDays=day});

            }
            return View(repoModel);
        }
    }
}

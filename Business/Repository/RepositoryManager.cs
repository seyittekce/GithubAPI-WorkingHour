
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Abstracts;
using Core.Entites;
using Microsoft.AspNetCore.Http;
using Octokit;
using Microsoft.AspNetCore.Http.Extensions;
namespace Business.Repository
{
    public class RepositoryManager : IRepositoryService
    {
        private readonly IGitHubClient _client;
        private readonly IWorkingHourCalculator _workingHourCalculator;
        private readonly HttpContext _httpContext;

        public RepositoryManager(IGitHubClient client, IWorkingHourCalculator workingHourCalculator, HttpContext httpContext)
        {
            _client = client;
            _workingHourCalculator = workingHourCalculator;
            this._httpContext = httpContext;
        }

        public async Task<List<RepositoryModel>> RepositoryWorkingDaysAsync()
        {
            var token = _httpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            var repositories = await _client.Repository.GetAllForCurrent();
            var repoModel = new List<RepositoryModel>();
            foreach (var item in repositories)
            {
                var issueComments = await _client.Issue.Comment.GetAllForRepository(item.Owner.Login, item.Name);
                var total = _workingHourCalculator.CalculateTotal(issueComments);
                var day = _workingHourCalculator.CalculateDayByDay(issueComments);
                repoModel.Add(new RepositoryModel { Repository = item, TotalWorkingHour = total, RepositoryWorkingDays = day });
            }

            return repoModel;
        }
    }
}

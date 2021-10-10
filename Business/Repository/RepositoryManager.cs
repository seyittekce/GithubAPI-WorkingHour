using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Abstracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Octokit;
namespace Business.Repository
{
    public class RepositoryManager : IRepositoryService
    {
        private readonly IGitHubClient _client;
        private readonly IWorkingHourCalculator _workingHourCalculator;
        private readonly IHttpContextAccessor _httpContext;
        public RepositoryManager(IGitHubClient client, IWorkingHourCalculator workingHourCalculator, IHttpContextAccessor httpContext)
        {
            _client = client;
            _workingHourCalculator = workingHourCalculator;
            _httpContext = httpContext;
        }
        public async Task<RepositoryModelWithMaxLength> RepositoryWorkingDaysAsync()
        {
            var token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            var repositories = await _client.Repository.GetAllForCurrent();
            var repoModel = new List<RepositoryModel>();
            foreach (var item in repositories)
            {
                var issueComments = await _client.Issue.Comment.GetAllForRepository(item.Owner.Login, item.Name);
    
                var total = _workingHourCalculator.CalculateTotal(issueComments);
                var day = _workingHourCalculator.CalculateDayByDay(issueComments);
                repoModel.Add(new RepositoryModel { Name = item.Name,Owner =item.Owner.Login ,TotalWorkingHour = total, RepositoryWorkingDays = day });
            }
            var returnList = new RepositoryModelWithMaxLength
            {
                Repository = repoModel,
                MaxLength = Enumerable.Count(Enumerable.FirstOrDefault(Enumerable.OrderByDescending(repoModel, x=>Enumerable.Count<RepositoryWorkingDays>(x.RepositoryWorkingDays))).RepositoryWorkingDays)


            };
            return returnList;
        }
    }
}

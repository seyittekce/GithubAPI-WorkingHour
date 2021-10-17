using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Abstracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Octokit;
namespace Business.User
{
    public class UserManager : IUserService
    {
        private readonly IGitHubClient _client;
        private readonly IWorkingHourCalculator _workingHourCalculator;
        private readonly IHttpContextAccessor _httpContext;
        public UserManager(IGitHubClient client, IWorkingHourCalculator workingHourCalculator, IHttpContextAccessor httpContext)
        {
            _client = client;
            _workingHourCalculator = workingHourCalculator;
            _httpContext = httpContext;
        }
        public async Task<List<IGrouping<string,GetRepoWithUser>>> GetRepositoryHourWithUsers(string owner, string repoName)
        {
            var token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            var getRepo = await _client.Repository.Get(owner, repoName);
            var getrepoList = new List<GetRepoWithUser>();
            var getIssue = await _client.Issue.Comment.GetAllForRepository(owner, repoName);
            foreach (var issue in getIssue)
            {
                getrepoList.Add(new GetRepoWithUser
                {
                    User = issue.User,
                    Date = issue.CreatedAt.Date,
                    Repository = getRepo,
                    UserName = issue.User.Login,
                    WorkingHour =_workingHourCalculator.CalculateTotal(issue) 
                });
            }

            var ss = getrepoList.GroupBy(x => x.UserName).ToList();

            return ss;
        }
    }
}
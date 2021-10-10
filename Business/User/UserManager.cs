using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Abstracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
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
        public async Task<GetRepositoryHourWithUserModel> GetRepositoryHourWithUsers(string owner, string repoName)
        {
            var token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            var getRepo = await _client.Repository.Get(owner, repoName);
            var getIssue = await _client.Issue.GetAllForRepository(owner, repoName);
          
            var getIssueComment =await _client.Issue.Comment.GetAllForRepository(owner,repoName);
            var selectIssueComment = getIssueComment.Select(x => new WorkingHourUserByDay
            {
                User = x.User,
                RepositoryWorkingDays = _workingHourCalculator.CalculateDayByDay(new List<IssueComment> {x}).ToList(),
                Total = _workingHourCalculator.CalculateTotal(new List<IssueComment> {x})
            }).GroupBy(x => x.User.Login);
                
            {
                var returning = new GetRepositoryHourWithUserModel
                {
                    Repository = getRepo,
                    WorkingHourUser = selectIssueComment

                };
               
              
                return returning;
            }
            return new GetRepositoryHourWithUserModel();
        }
    }
}
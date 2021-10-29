using Core.Abstracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<UserModelForRepositoryWithDates> GetRepositoryHourWithUsers(string owner, string repoName, DateTime? startDate, DateTime? endDate)
        {
            string token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            List<PlaneUserModel> userList = new List<PlaneUserModel>();
            UserModelForRepositoryWithDates returnList = new UserModelForRepositoryWithDates();
            Octokit.Repository getRepository = await _client.Repository.Get(owner, repoName);
            IReadOnlyList<IssueComment> getIssueComment = await _client.Issue.Comment.GetAllForRepository(owner, repoName);
            foreach (IssueComment comment in getIssueComment)
            {
                if ((startDate.HasValue && endDate.HasValue))
                {
                    if (comment.CreatedAt.Date >= startDate && comment.CreatedAt.Date <= endDate)
                    {
                        userList.Add(new PlaneUserModel { Date = comment.CreatedAt.Date, User = comment.User, WorkingMinutes = _workingHourCalculator.CalculateTotal(comment) });
                    }
                }
                else
                {
                    userList.Add(new PlaneUserModel { Date = comment.CreatedAt.Date, User = comment.User, WorkingMinutes = _workingHourCalculator.CalculateTotal(comment) });
                }
            }
            List<IGrouping<DateTime, PlaneUserModel>> model = userList.GroupBy(x => x.Date.Date).OrderBy(x => x.Key).ToList();
            for (DateTime date = model.Min(x => x.Key); date <= model.Max(x => x.Key); date = date.Date.AddDays(1))
            {
                returnList.Dates.Add(date);
            }
            foreach (IGrouping<string, PlaneUserModel> list in userList.GroupBy(x => x.User.Login))
            {
                Octokit.User user = list.FirstOrDefault().User;
                IEnumerable<string> userDates = list.ToList().Select(x => x.Date.Date.ToShortDateString());
                List<DateTime> outCasted = returnList.Dates.Where(x => !userDates.Contains(x.Date.ToShortDateString())).ToList();
                foreach (DateTime date in outCasted)
                {
                    userList.Add(new PlaneUserModel { Date = date, WorkingMinutes = 0, User = user });
                }
            }
            returnList.Repository = getRepository;
            foreach (IGrouping<string, PlaneUserModel> item in userList.GroupBy(x => x.User.Login))
            {
                Octokit.User user = item.FirstOrDefault().User;
                int totalWorkingHour = item.Sum(x => x.WorkingMinutes);
                TimeSpan date = new TimeSpan(0, totalWorkingHour, 0);
                List<WorkingDays> list = new List<WorkingDays>();
                foreach (PlaneUserModel workingHour in item.ToList())
                {
                    WorkingDays isExistBefore = list.Where(x => x.Date.Date == workingHour.Date.Date).FirstOrDefault();
                    if (isExistBefore != null)
                    {
                        isExistBefore.TotalWorkingMinutes += workingHour.WorkingMinutes;
                    }
                    else
                    {
                        list.Add(new WorkingDays { Date = workingHour.Date.Date, TotalWorkingMinutes = workingHour.WorkingMinutes });
                    }
                }
                returnList.UserWorkingHours.Add(new UserWorkingHour
                {
                    User = user,
                    RepositoryWorkingDays = list,
                    TotalWorkingHour = string.Format("{0:00} {1:00}", (int)date.TotalHours, date.Minutes)
                });
            }
            return returnList;
        }
        public async Task<UserModelWithDates> GetUserWorkingHour(DateTime? startDate, DateTime? endDate)
        {
            string token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            List<PlaneUserModel> repoModelList = new List<PlaneUserModel>();
            UserModelWithDates returnList = new UserModelWithDates();
            List<DateTime> dates = new();
            IReadOnlyList<Octokit.Repository> repositories = await _client.Repository.GetAllForCurrent();
            foreach (Octokit.Repository item in repositories)
            {
                IReadOnlyList<IssueComment> getRepoComments = await _client.Issue.Comment.GetAllForRepository(item.Owner.Login, item.Name);
                foreach (IssueComment comment in getRepoComments)
                {
                    if ((startDate.HasValue && endDate.HasValue))
                    {
                        if (comment.CreatedAt.Date >= startDate && comment.CreatedAt.Date <= endDate)
                        {
                            PlaneUserModel planeRepositoryModel = new PlaneUserModel
                            {
                                Repository = item,
                                Date = comment.CreatedAt.DateTime,
                                WorkingMinutes = _workingHourCalculator.CalculateTotal(comment),
                                User = comment.User
                            };
                            repoModelList.Add(planeRepositoryModel);
                        }
                    }
                    else
                    {
                        PlaneUserModel planeRepositoryModel = new PlaneUserModel
                        {
                            Repository = item,
                            Date = comment.CreatedAt.DateTime,
                            WorkingMinutes = _workingHourCalculator.CalculateTotal(comment),
                            User = comment.User
                        };
                        repoModelList.Add(planeRepositoryModel);
                    }

                }
            }

            List<IGrouping<DateTime, PlaneUserModel>> model = repoModelList.GroupBy(x => x.Date.Date).OrderBy(x => x.Key).ToList();
            for (DateTime date = model.Min(x => x.Key); date <= model.Max(x => x.Key); date = date.Date.AddDays(1))
            {
                dates.Add(date);
            }
            returnList.Dates = dates;
            foreach (IGrouping<string, PlaneUserModel> repo in repoModelList.GroupBy(x => x.User.Login))
            {
                IEnumerable<string> repoDates = repo.ToList().Select(x => x.Date.Date.ToShortDateString());
                List<DateTime> outCasted = dates.Where(x => !repoDates.Contains(x.Date.ToShortDateString())).ToList();
                foreach (DateTime date in outCasted)
                {
                    repoModelList.Add(new PlaneUserModel { Date = date, Repository = repo.FirstOrDefault()?.Repository, WorkingMinutes = 0, User = repo.FirstOrDefault().User });
                }
            }
            foreach (IGrouping<string, PlaneUserModel> item in repoModelList.GroupBy(x => x.User.Login))
            {
                Octokit.Repository repoName = item.FirstOrDefault().Repository;
                int totalWorkingHour = item.Sum(x => x.WorkingMinutes);
                TimeSpan date = new TimeSpan(0, totalWorkingHour, 0);
                List<WorkingDays> list = new List<WorkingDays>();
                foreach (PlaneUserModel workingHour in item.ToList())
                {
                    WorkingDays isExistBefore = list.Where(x => x.Date.Date == workingHour.Date.Date).FirstOrDefault();
                    if (isExistBefore != null)
                    {
                        isExistBefore.TotalWorkingMinutes += workingHour.WorkingMinutes;
                    }
                    else
                    {
                        list.Add(new WorkingDays { Date = workingHour.Date.Date, TotalWorkingMinutes = workingHour.WorkingMinutes });
                    }
                }
                returnList.User.Add(new UserModel
                {
                    User = item.FirstOrDefault().User,
                    RepositoryWorkingDays = list,
                    TotalWorkingHour = string.Format("{0:00} {1:00}", (int)date.TotalHours, date.Minutes)
                });
            }
            return returnList;
        }
    }
}
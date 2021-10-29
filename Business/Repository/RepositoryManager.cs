using Core.Abstracts;
using Core.Entities;
using Core.Repository;
using Microsoft.AspNetCore.Http;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<RepositoryModelWithDates> RepositoryWorkingDaysAsync(DateTime? startDate, DateTime? endDate)
        {
            string token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            List<PlaneRepositoryModel> repoModelList = new List<PlaneRepositoryModel>();
            RepositoryModelWithDates returnList = new RepositoryModelWithDates();
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
                            PlaneRepositoryModel planeRepositoryModel = new PlaneRepositoryModel
                            {
                                Repository = item,
                                Date = comment.CreatedAt.DateTime,
                                WorkingMinutes = _workingHourCalculator.CalculateTotal(comment)
                            };
                            repoModelList.Add(planeRepositoryModel);
                        }
                    }
                    else
                    {
                        PlaneRepositoryModel planeRepositoryModel = new PlaneRepositoryModel
                        {
                            Repository = item,
                            Date = comment.CreatedAt.DateTime,
                            WorkingMinutes = _workingHourCalculator.CalculateTotal(comment)
                        };
                        repoModelList.Add(planeRepositoryModel);
                    }

                }
            }

            List<IGrouping<DateTime, PlaneRepositoryModel>> model = repoModelList.GroupBy(x => x.Date.Date).OrderBy(x => x.Key).ToList();
            for (DateTime date = model.Min(x => x.Key); date <= model.Max(x => x.Key); date = date.Date.AddDays(1))
            {
                dates.Add(date);
            }
            returnList.Dates = dates;
            foreach (IGrouping<string, PlaneRepositoryModel> repo in repoModelList.GroupBy(x => x.Repository.Name))
            {
                IEnumerable<string> repoDates = repo.ToList().Select(x => x.Date.Date.ToShortDateString());
                List<DateTime> outCasted = dates.Where(x => !repoDates.Contains(x.Date.ToShortDateString())).ToList();
                foreach (DateTime date in outCasted)
                {
                    repoModelList.Add(new PlaneRepositoryModel { Date = date, Repository = repo.FirstOrDefault()?.Repository, WorkingMinutes = 0 });
                }
            }
            foreach (IGrouping<string, PlaneRepositoryModel> item in repoModelList.GroupBy(x => x.Repository.Name))
            {
                Octokit.Repository repoName = item.FirstOrDefault().Repository;
                int totalWorkingHour = item.Sum(x => x.WorkingMinutes);
                TimeSpan date = new TimeSpan(0, totalWorkingHour, 0);
                List<WorkingDays> list = new List<WorkingDays>();
                foreach (PlaneRepositoryModel workingHour in item.ToList())
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
                returnList.Repository.Add(new RepositoryModel
                {
                    Repository = repoName,
                    RepositoryWorkingDays = list,
                    TotalWorkingHour = string.Format("{0:00} {1:00}", (int)date.TotalHours, date.Minutes)
                });
            }
            return returnList;
        }
    }
}

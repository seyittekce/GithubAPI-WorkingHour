using System;
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
        public async Task<RepositoryModelWithDates> RepositoryWorkingDaysAsync(DateTime? startDate, DateTime? endDate)
        {
            var token = _httpContext.HttpContext.Session.GetString("OAuthToken");
            _client.Connection.Credentials = new Credentials(token);
            var repoModelList = new List<PlaneRepositoryModel>();
            var returnList = new RepositoryModelWithDates();
            List<DateTime> dates = new();
            var repositories = await _client.Repository.GetAllForCurrent();
            foreach (var item in repositories)
            {
                var getRepoComments = await _client.Issue.Comment.GetAllForRepository(item.Owner.Login, item.Name);
                foreach (var comment in getRepoComments)
                {
                    if ((startDate.HasValue&&endDate.HasValue))
                    {
                        if (comment.CreatedAt.Date >= startDate && comment.CreatedAt.Date <= endDate)
                        {
                            var planeRepositoryModel = new PlaneRepositoryModel();
                            planeRepositoryModel.Repository = item;
                            planeRepositoryModel.Date = comment.CreatedAt.DateTime;
                            planeRepositoryModel.WorkingMinutes = _workingHourCalculator.CalculateTotal(comment);
                            repoModelList.Add(planeRepositoryModel);
                        }
                    }
                    else
                    {
                        var planeRepositoryModel = new PlaneRepositoryModel();
                        planeRepositoryModel.Repository = item;
                        planeRepositoryModel.Date = comment.CreatedAt.DateTime;
                        planeRepositoryModel.WorkingMinutes = _workingHourCalculator.CalculateTotal(comment);
                        repoModelList.Add(planeRepositoryModel);
                    }
                
                }
            }
          
            var model = repoModelList.GroupBy(x => x.Date.Date).OrderBy(x => x.Key).ToList();
            for (var date = model.Min(x => x.Key); date <= model.Max(x => x.Key); date = date.Date.AddDays(1))
            {
                dates.Add(date);
            }
            returnList.Dates = dates;
            foreach (var repo in repoModelList.GroupBy(x => x.Repository.Name))
            {
                var repoDates = repo.ToList().Select(x => x.Date.Date.ToShortDateString());
                var outCasted = dates.Where(x => !repoDates.Contains(x.Date.ToShortDateString())).ToList();
                foreach (var date in outCasted)
                {
                    repoModelList.Add(new PlaneRepositoryModel { Date = date, Repository = repo.FirstOrDefault()?.Repository, WorkingMinutes = 0 });
                }
            }
            foreach (var item in repoModelList.GroupBy(x => x.Repository.Name))
            {
                var repoName = item.FirstOrDefault().Repository;
                var totalWorkingHour = item.Sum(x => x.WorkingMinutes);
                var date = new TimeSpan(0, totalWorkingHour, 0);
                var list = new List<RepositoryWorkingDays>();
                foreach (var workingHour in item.ToList())
                {
                    var isExistBefore = list.Where(x => x.Date.Date == workingHour.Date.Date).FirstOrDefault();
                    if (isExistBefore != null)
                    {
                        isExistBefore.TotalWorkingMinutes += workingHour.WorkingMinutes;
                    }
                    else
                    {
                        list.Add(new RepositoryWorkingDays { Date = workingHour.Date.Date, TotalWorkingMinutes = workingHour.WorkingMinutes });
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

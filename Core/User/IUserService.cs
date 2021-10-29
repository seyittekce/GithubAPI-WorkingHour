using Core.Entities;
using System;
using System.Threading.Tasks;

namespace Business.User
{
    public interface IUserService
    {
        Task<UserModelForRepositoryWithDates> GetRepositoryHourWithUsers(string owner, string repoName, DateTime? startDate, DateTime? endDate);
        Task<UserModelWithDates> GetUserWorkingHour(DateTime? startDate, DateTime? endDate);
    }
}
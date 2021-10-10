using System.Threading.Tasks;

using Core.Entities;
using Octokit;

namespace Business.User
{
    public interface IUserService
    {
        Task<GetRepositoryHourWithUserModel> GetRepositoryHourWithUsers(string owner,string repoName);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Entities;
using Octokit;

namespace Business.User
{
    public interface IUserService
    {
        Task<List<IGrouping<string, GetRepoWithUser>>> GetRepositoryHourWithUsers(string owner,string repoName);
    }
}
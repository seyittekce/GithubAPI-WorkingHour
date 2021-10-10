using System.Collections.Generic;
using System.Linq;
using Octokit;
namespace Core.Entities
{
    public class GetRepositoryHourWithUserModel
    {
        public Repository Repository { get; set; }
        public IEnumerable<IGrouping<string, WorkingHourUserByDay>> WorkingHourUser { get; set; }
    }
}
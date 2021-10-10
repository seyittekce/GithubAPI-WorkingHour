
using System.Collections.Generic;
using Core.Entities;

namespace GithubAPI_WorkingHour.Models
{
    public class RepositoryModel
    {
        public Octokit.Repository Repository { get; set; }
        public string TotalWorkingHour { get; set; } = "0";
        public IEnumerable<RepositoryWorkingDays> RepositoryWorkingDays { get; set; }
    }
}
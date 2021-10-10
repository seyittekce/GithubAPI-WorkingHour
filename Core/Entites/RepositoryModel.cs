using System.Collections.Generic;
namespace Core.Entites
{
    public class RepositoryModel
    {
        public Octokit.Repository Repository { get; set; }
        public string TotalWorkingHour { get; set; } = "0";
        public IEnumerable<RepositoryWorkingDays> RepositoryWorkingDays { get; set; }
    }
}

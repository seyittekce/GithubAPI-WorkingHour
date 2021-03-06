using System.Collections.Generic;
namespace Core.Entities
{
    public class RepositoryModel
    {
        public Octokit.Repository Repository { get; set; }
        public string TotalWorkingHour { get; set; } = "0";
        public IEnumerable<WorkingDays> RepositoryWorkingDays { get; set; } =
            new List<WorkingDays>();
    }
}

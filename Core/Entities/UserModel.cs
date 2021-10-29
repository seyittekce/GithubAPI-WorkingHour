using System.Collections.Generic;
namespace Core.Entities
{
    public class UserModel
    {
        public Octokit.User User { get; set; }
        public string TotalWorkingHour { get; set; } = "0";
        public IEnumerable<WorkingDays> RepositoryWorkingDays { get; set; } =
            new List<WorkingDays>();
    }
}

using System;
using System.Collections.Generic;
namespace Core.Entities
{
    public class UserModelForRepositoryWithDates
    {
        public Octokit.Repository Repository { get; set; }
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public List<UserWorkingHour> UserWorkingHours { get; set; } =
            new List<UserWorkingHour>();
    }
    public class UserWorkingHour
    {
        public Octokit.User User { get; set; }
        public string TotalWorkingHour { get; set; }
        public List<WorkingDays> RepositoryWorkingDays { get; set; } =
          new List<WorkingDays>();
    }
}

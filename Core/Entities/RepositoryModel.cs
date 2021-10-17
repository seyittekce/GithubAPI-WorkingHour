using System;
using System.Collections.Generic;
namespace Core.Entities
{
    public class RepositoryModel
    {
        public Octokit.Repository Repository { get; set; }
        public string TotalWorkingHour { get; set; } = "0";
        public IEnumerable<RepositoryWorkingDays> RepositoryWorkingDays { get; set; } =
            new List<RepositoryWorkingDays>();
    }

    public class RepositoryModelWithDates
    {
        public List<RepositoryModel> Repository {  get; set; }=new List<RepositoryModel>();
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
    }
}

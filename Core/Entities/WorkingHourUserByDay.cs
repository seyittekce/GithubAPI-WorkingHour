using System;
using System.Collections.Generic;
using System.Linq;
using Octokit;

namespace Core.Entities
{
    public class WorkingHourUserByDay
    {   
        public User User { get; set; }
        public IEnumerable<RepositoryWorkingDays> RepositoryWorkingDays { get; set; }
        public string Total { get; set; }
        public string UserName { get; set; }
        
    }
}
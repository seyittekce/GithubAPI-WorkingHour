using System;
using System.Collections.Generic;
using Octokit;

namespace Core.Entities
{
    public class GetRepoWithUser
    {
        public Repository Repository {  get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public double WorkingHour{ get; set; }
        public string UserName { get; set; }    
    }
}
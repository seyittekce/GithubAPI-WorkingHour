using Octokit;
using System.Collections.Generic;

namespace GithubAPI_WorkingHour.Models.Repository
{
    public class IssuesListWithComment
    {
        public Issue Issue { get; set; }
        public IReadOnlyList<IssueComment> IssueComments { get; set; }
    }
}
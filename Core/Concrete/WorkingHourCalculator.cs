using Core.Abstracts;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Entities;
namespace Core.Concrete
{
    public class WorkingHourCalculator : IWorkingHourCalculator
    {

        public int CalculateTotal(IssueComment issue)
        {
            Regex regex = new Regex("\\{[^}]*\\}", RegexOptions.IgnoreCase);
            int hour = 0;
            int munites = 0;
            var match = regex.Match(issue.Body);
            if (!string.IsNullOrEmpty(match.Value))
            {
                var subMatch = match.Value.Replace("{", "").Replace("}", "");
                hour = int.Parse(subMatch.Substring(0, subMatch.IndexOf("h", StringComparison.Ordinal)));
                munites = int.Parse(subMatch.Substring(subMatch.IndexOf("h", StringComparison.Ordinal) + 1).Replace("m", "").Replace(" ", ""));
            }
            return (munites + hour * 60);
        }
        public string CalculateTotal(IEnumerable<IssueComment> comments)
        {
            Regex regex = new Regex("\\{[^}]*\\}", RegexOptions.IgnoreCase);
            int hour = 0;
            int munites = 0;
            foreach (var comment in comments)
            {
                var match = regex.Match(comment.Body);
                if (!string.IsNullOrEmpty(match.Value))
                {
                    var subMatch = match.Value.Replace("{", "").Replace("}", "");
                    hour += int.Parse(subMatch.Substring(0, subMatch.IndexOf("h", StringComparison.Ordinal)));
                    munites += int.Parse(subMatch.Substring(subMatch.IndexOf("h", StringComparison.Ordinal) + 1).Replace("m", "").Replace(" ", ""));
                }
            }
            var calcMunites = TimeSpan.FromMinutes(munites + hour * 60);
            return string.Format("{0:00}:{1:00}", (int)calcMunites.TotalHours, calcMunites.Minutes);
        }
    }
}
using Core.Abstracts;
using Core.Entites;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Concrete
{
    public class WorkingHourCalculator : IWorkingHourCalculator
    {
        public List<RepositoryWorkingDays> CalculateDayByDay(IEnumerable<IssueComment> comments)
        {
            return comments.GroupBy(x => x.CreatedAt.Date).Select(a => new RepositoryWorkingDays
            {

                TotalWorkingHour = CalculateTotal(a.ToList()),
                Date = a.Key.Date
            }).ToList();


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

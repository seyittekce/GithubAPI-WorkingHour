using Core.Entites;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Abstracts
{
    public interface IWorkingHourCalculator
    {
        
        string CalculateTotal(IEnumerable<IssueComment> issue);
        List<RepositoryWorkingDays> CalculateDayByDay(IEnumerable<IssueComment> comments);

    }
}

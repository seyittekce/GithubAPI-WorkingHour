using Core.Entites;
using Octokit;
using System.Collections.Generic;

namespace Core.Abstracts
{
    public interface IWorkingHourCalculator
    {
        string CalculateTotal(IEnumerable<IssueComment> issue);

        List<RepositoryWorkingDays> CalculateDayByDay(IEnumerable<IssueComment> comments);
    }
}
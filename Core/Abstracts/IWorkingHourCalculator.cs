using Octokit;

namespace Core.Abstracts
{
    public interface IWorkingHourCalculator
    {
        int CalculateTotal(IssueComment issue);


    }
}
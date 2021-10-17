using Octokit;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Abstracts
{
    public interface IWorkingHourCalculator
    {
        int CalculateTotal(IssueComment issue);

       
    }
}
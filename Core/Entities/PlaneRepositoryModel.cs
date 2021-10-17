using System;
using Octokit;
namespace Core.Entities
{
    public class PlaneRepositoryModel
    {
        public Repository Repository { get; set; }
        public DateTime Date { get; set; }
        public int WorkingMinutes { get; set; }
        public string WorkingHourWithMinutes
        {
            get
            {
                var date = new TimeSpan(0, WorkingMinutes, 0);
                return string.Format("{0:00} {1:00}", (int)date.TotalHours, date.Minutes);
            }
        }
    }
}
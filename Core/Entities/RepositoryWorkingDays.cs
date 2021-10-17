using System;
namespace Core.Entities
{
    public class RepositoryWorkingDays
    {
        public int TotalWorkingMinutes { get; set; } =0;
        public DateTime Date { get; set; } = new();
    }
}
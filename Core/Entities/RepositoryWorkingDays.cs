using System;
namespace Core.Entities
{
    public class WorkingDays
    {
        public int TotalWorkingMinutes { get; set; } = 0;
        public DateTime Date { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
namespace Core.Entities
{
    public class RepositoryModelWithDates
    {
        public List<RepositoryModel> Repository { get; set; } = new List<RepositoryModel>();
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
    }
}

using System;
using System.Collections.Generic;
namespace Core.Entities
{
    public class UserModelWithDates
    {
        public List<UserModel> User { get; set; } = new();
        public List<DateTime> Dates { get; set; } = new();
    }
}

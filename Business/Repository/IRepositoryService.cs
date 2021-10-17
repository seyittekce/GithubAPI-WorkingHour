
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Business.Repository
{
    public interface IRepositoryService
    {
        Task<RepositoryModelWithDates> RepositoryWorkingDaysAsync(DateTime? startDate,DateTime? endDate);


    }
}

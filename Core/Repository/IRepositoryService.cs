using Core.Entities;
using System;
using System.Threading.Tasks;
namespace Core.Repository
{
    public interface IRepositoryService
    {
        Task<RepositoryModelWithDates> RepositoryWorkingDaysAsync(DateTime? startDate, DateTime? endDate);


    }
}

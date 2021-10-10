using Core.Entites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Business.Repository
{
    public interface IRepositoryService
    {
        Task<List<RepositoryModel>> RepositoryWorkingDaysAsync();


    }
}

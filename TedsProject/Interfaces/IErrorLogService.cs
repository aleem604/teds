using System;
using System.Threading.Tasks;
using TedsProject.Models;

namespace TedsProject.Interfaces
{
    public interface IErrorLogService
    {
        Task<dynamic> GetAll(DateTime startDate, DateTime endDate);
        Task SaveExceptionLog(Exception ex, string methodName = "");
    }
}
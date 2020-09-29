using System;
using System.Threading.Tasks;
using TedsProject.Models;

namespace TedsProject.Interfaces
{
    public interface ILoggingService
    {
        Task<dynamic> GetAll(DateTime startDate, DateTime endDate);
        Task SaveLog(LoggingModel loggingModel, string methodName);
    }
}
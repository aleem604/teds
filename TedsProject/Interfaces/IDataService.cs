using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TedsProject.Models;

namespace TedsProject.Interfaces
{
    public interface IDataService
    {
        Task<dynamic> CreateTable();
        Task<dynamic> GetAll();
        Task<dynamic> GetById(string key);
        Task<dynamic> SaveItem(CrossingsModel model);
        Task<dynamic> SearchBYLatLang(decimal lat, decimal lng, string key);
        Task<dynamic> DeleteCrossing(string key);
        Task<dynamic> GetGateStatus(string id);
        Task<dynamic> UpdateGateStatus(bool isOpen, string id);
    }
}

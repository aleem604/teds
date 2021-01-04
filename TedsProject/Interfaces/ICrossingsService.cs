using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TedsProject.Models;

namespace TedsProject.Interfaces
{
    public interface ICrossingsService
    {
        Task<dynamic> UploadCrossings(IFormFile file);
        Task<dynamic> UploadCrossingsNew(IFormFile file);
        Task<dynamic> GetAll(string country, string tucNumber);
        Task<dynamic> GetById(string key);
        Task<dynamic> SaveItem(CrossingsModel model);
        Task<dynamic> SearchBYLatLang(decimal lat, decimal lng);
        Task<dynamic> SearchBYRadius(double lat, double lng, int radius);
        Task<dynamic> DeleteCrossing(string key);
        Task<dynamic> DeleteAllCrossings();
        Task<dynamic> DeleteNewCrossings();
        Task<dynamic> GetGateStatus(string id);
        Task<dynamic> UpdateGateStatus(bool isOpen, string id);
        
        Task<dynamic> GetGateStatusByTCNumber(string country, string tcnumber);
        Task<dynamic> UpdateGateStatusByTCNumber(bool isOpen, string country, string tcnumber);
    }
}

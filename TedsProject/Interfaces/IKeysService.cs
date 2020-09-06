using System.Threading.Tasks;
using TedsProject.Models;

namespace TedsProject.Interfaces
{
    public interface IKeysService
    {
        Task<dynamic> GetAll();
        Task<dynamic> GetById(string key);
        Task<dynamic> GetAppkeyByUser(string userId);
        Task<dynamic> SaveItem(string userId);
        Task<dynamic> DeleteKey(string key);
        Task<bool> ValidateAppKey(string key);
    }
}
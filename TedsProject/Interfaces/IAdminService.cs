using System.Threading.Tasks;
using TedsProject.Models;

namespace TedsProject.Interfaces
{
    public interface IAdminService
    {
        Task<(dynamic, string)> Login(LoginViewModel model);
        Task<(dynamic, string)> Register(SignupViewModel model);
        Task<(dynamic, string)> ChangePassword(ChangePasswordViewModel model, string name);
    }
}
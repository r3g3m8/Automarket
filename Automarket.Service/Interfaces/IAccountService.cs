using System.Security.Claims;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Account;

namespace Automarket.Service.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<string>> SendConfirmationCode(string email);
        Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model);

        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);

        Task<BaseResponse<bool>> ChangePassword(ChangePasswordViewModel model);
    }
}

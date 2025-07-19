using StudentAPIBusinessLayer.DTO.Auth;

namespace StudentAPIBusinessLayer.Interfaces
{
    public interface IAuthService
    {
       Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokentAsync(TokenRequestModel token);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}

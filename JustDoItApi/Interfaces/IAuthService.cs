using JustDoItApi.Models.Auth;

namespace JustDoItApi.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginModel model);
    Task<string> RegisterAsync(RegisterModel model);
    Task<string> EditProfileAsync(EditProfileModel model);
}

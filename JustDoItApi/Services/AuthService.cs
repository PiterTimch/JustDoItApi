using AutoMapper;
using JustDoItApi.Entities.Identity;
using JustDoItApi.Interfaces;
using JustDoItApi.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace JustDoItApi.Services;

public class AuthService(IJWTTokenService tokenService,
        UserManager<UserEntity> userManager,
        IMapper mapper,
        IImageService imageService) : IAuthService
{
    public async Task<string> LoginAsync(LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = await tokenService.CreateTokenAsync(user);
            user.IsDeleted = false;
            await userManager.UpdateAsync(user);
            return token;
        }
        return string.Empty;
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var user = mapper.Map<UserEntity>(model);
        if (model.ImageFile != null)
        {
            user.Image = await imageService.SaveImageAsync(model.ImageFile);
        }
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
            var token = await tokenService.CreateTokenAsync(user);
            return token;
        }
        return string.Empty;
    }
}

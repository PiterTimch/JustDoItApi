using AutoMapper;
using JustDoItApi.Entities.Identity;
using JustDoItApi.Interfaces;
using JustDoItApi.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace JustDoItApi.Services;

public class AuthService(
    IJWTTokenService tokenService,
    UserManager<UserEntity> userManager,
    RoleManager<RoleEntity> roleManager,
    IMapper mapper,
    IImageService imageService,
    IIdentityService identityService
) : IAuthService
{
    private const string DefaultRole = "User";

    public async Task<string> LoginAsync(LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return string.Empty;

        var isValidPassword = await userManager.CheckPasswordAsync(user, model.Password);
        if (!isValidPassword)
            return string.Empty;

        user.IsDeleted = false;
        await userManager.UpdateAsync(user);

        return await tokenService.CreateTokenAsync(user);
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var user = mapper.Map<UserEntity>(model);

        if (model.ImageFile != null)
        {
            user.Image = await imageService.SaveImageAsync(model.ImageFile);
        }

        var createResult = await userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
            return string.Empty;

        if (!await roleManager.RoleExistsAsync(DefaultRole))
        {
            await roleManager.CreateAsync(new RoleEntity
            {
                Name = DefaultRole
            });
        }

        await userManager.AddToRoleAsync(user, DefaultRole);
        return await tokenService.CreateTokenAsync(user);
    }

    public async Task<string> EditProfileAsync(EditProfileModel model)
    {
        var userId = await identityService.GetUserIdAsync();

        var user = await userManager.FindByIdAsync(Convert.ToString(userId));

        if (user == null)
            throw new Exception("User not found");
        mapper.Map(model, user);
        if (model.ImageFile != null)
        {
            if (!string.IsNullOrEmpty(user.Image))
                await imageService.DeleteImageAsync(user.Image);

            user.Image = await imageService.SaveImageAsync(model.ImageFile);
        }
        await userManager.UpdateAsync(user);

        return await tokenService.CreateTokenAsync(user);
    }
}

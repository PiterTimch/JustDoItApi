using AutoMapper;
using JustDoItApi.Data;
using JustDoItApi.Entities;
using JustDoItApi.Entities.Identity;
using JustDoItApi.Interfaces;
using JustDoItApi.Models.Zadachi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JustDoItApi.Services;

public class ZadachiService(
    AppDbContext context,
    IMapper mapper,
    IImageService imageService,
    IIdentityService identityService
) : IZadachiService
{
    public async Task<ZadachaItemModel> CreateZadachyAsync(ZadachaCreateModel model)
    {
        long userId = await identityService.GetUserIdAsync();

        var zadachaEntity = mapper.Map<ZadachaEntity>(model);
        zadachaEntity.UserId = userId;
        zadachaEntity.Image = await imageService.SaveImageAsync(model.Image);

        context.Zadachi.Add(zadachaEntity);
        await context.SaveChangesAsync();

        return mapper.Map<ZadachaItemModel>(zadachaEntity);
    }

    public async Task<IEnumerable<ZadachaItemModel>> GetAllAsync()
    {
        long userId = await identityService.GetUserIdAsync();

        var zadachi = await context.Zadachi
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return mapper.Map<IEnumerable<ZadachaItemModel>>(zadachi);
    }

    public async Task<bool> DeleteZadachyAsync(long id)
    {
        long userId = await identityService.GetUserIdAsync();

        var zadachaEntity = await context.Zadachi
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (zadachaEntity == null)
            return false;

        await imageService.DeleteImageAsync(zadachaEntity.Image);

        context.Zadachi.Remove(zadachaEntity);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteRangeZadachiAsync(List<long> ids)
    {
        long userId = await identityService.GetUserIdAsync();

        var zadachiEntities = await context.Zadachi
            .Where(x => ids.Contains(x.Id) && x.UserId == userId)
            .ToListAsync();

        if (zadachiEntities.Count == 0)
            return false;

        foreach (var zadacha in zadachiEntities)
        {
            await imageService.DeleteImageAsync(zadacha.Image);
        }

        context.Zadachi.RemoveRange(zadachiEntities);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateZadachyAsync(ZadachaUpdateModel model)
    {
        long userId = await identityService.GetUserIdAsync();

        var zadachaEntity = await context.Zadachi
            .FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);

        if (zadachaEntity == null)
            return false;

        mapper.Map(model, zadachaEntity);

        if (model.Image != null)
        {
            await imageService.DeleteImageAsync(zadachaEntity.Image);
            zadachaEntity.Image = await imageService.SaveImageAsync(model.Image);
        }

        await context.SaveChangesAsync();
        return true;
    }
}


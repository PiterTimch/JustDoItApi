using AutoMapper;
using JustDoItApi.Data;
using JustDoItApi.Entities.Chat;
using JustDoItApi.Interfaces;
using JustDoItApi.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace JustDoItApi.Services;

public class ChatService(
    AppDbContext context,
    IIdentityService identityService,
    IMapper mapper) : IChatService
{
    public async Task<long> CreateChatAsync(ChatCreateModel model)
    {
        var userId = await identityService.GetUserIdAsync();

        var chat = mapper.Map<ChatEntity>(model, opt =>
        {
            opt.Items["UserId"] = userId;
        });

        context.Chats.Add(chat);
        await context.SaveChangesAsync();

        return chat.Id;
    }

    public Task<List<ChatTypeItemModel>> GetAllTypes()
    {
        return context.ChatTypes
            .AsNoTracking()
            .Select(ct => mapper.Map<ChatTypeItemModel>(ct))
            .ToListAsync();
    }

    public async Task<ChatMessageModel> SendMessageAsync(SendMessageModel model)
    {
        var userId = await identityService.GetUserIdAsync();

        var isMember = await context.ChatUsers
            .AnyAsync(x => x.ChatId == model.ChatId && x.UserId == userId);

        if (!isMember)
            throw new UnauthorizedAccessException("User is not in chat");

        var message = new ChatMessageEntity
        {
            ChatId = model.ChatId,
            UserId = userId,
            Message = model.Message
        };

        context.ChatMessages.Add(message);
        await context.SaveChangesAsync();

        return mapper.Map<ChatMessageModel>(message);
    }

    public async Task<bool> IsUserInChat(long chatId, long userId)
    {
        return await context.ChatUsers
            .AnyAsync(x => x.ChatId == chatId && x.UserId == userId);
    }

    public async Task<List<UserShortModel>> GetAllUsersAsync()
    {
        var users = await context.Users
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<UserShortModel>>(users);
    }

    public async Task<List<ChatListItemModel>> GetMyChatsAsync()
    {
        var userId = await identityService.GetUserIdAsync();

        var chats = await context.ChatUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == userId)
            .Select(cu => cu.Chat)
            .ToListAsync();

        return mapper.Map<List<ChatListItemModel>>(chats);
    }

    public async Task<List<ChatMessageModel>> GetChatMessagesAsync(long chatId)
    {
        var userId = await identityService.GetUserIdAsync();
        var isMember = await context.ChatUsers
            .AnyAsync(x => x.ChatId == chatId && x.UserId == userId);

        if (!isMember) throw new UnauthorizedAccessException();

        var messages = await context.ChatMessages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Id)
            .ToListAsync();

        return mapper.Map<List<ChatMessageModel>>(messages);
    }

}
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

    public async Task<List<UserShortModel>> GetAllUsersAsync(UserSearchModel model)
    {
        var query = context.Users
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.Query))
        {
            var search = model.Query.Trim().ToLower();

            query = query.Where(u =>
                (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(search)) ||
                (
                    u.FirstName != null &&
                    u.LastName != null &&
                    (u.FirstName + " " + u.LastName).ToLower().Contains(search)
                )
            );
        }

        if (model.ChatId.HasValue)
        {
            var chatUserIds = context.ChatUsers
                .Where(cu => cu.ChatId == model.ChatId.Value)
                .Select(cu => cu.UserId);

            query = query.Where(u => chatUserIds.Contains(u.Id));
        }

        var users = await query.ToListAsync();
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

        var entities = await context.ChatMessages
            .Include(m => m.User)
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Id)
            .ToListAsync();

        var messages = mapper.Map<List<ChatMessageModel>>(entities);

        return messages;
    }

    public async Task<bool> AmIAdminAsync(long chatId)
    {
        var chat = await context.Chats
            .AsNoTracking()
            .Include(c => c.ChatUsers)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new KeyNotFoundException("Chat not found");

        var chatAdminId = chat.ChatUsers!
            .FirstOrDefault(cu => cu.IsAdmin)?.UserId;

        var userId = await identityService.GetUserIdAsync();

        return chatAdminId == userId;
    }

    public async Task EditChatAsync(ChatEditModel model)
    {
        var currentUserId = await identityService.GetUserIdAsync();

        var chat = await context.Chats
            .Include(c => c.ChatUsers)
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (chat == null)
            throw new KeyNotFoundException("Chat not found");

        var isAdmin = chat.ChatUsers
            .Any(cu => cu.UserId == currentUserId && cu.IsAdmin);

        if (!isAdmin)
            throw new UnauthorizedAccessException("User is not admin of the chat");

        if (!string.IsNullOrWhiteSpace(model.Name))
        {
            chat.Name = model.Name.Trim();
        }

        if (model.AddUserIds?.Any() == true)
        {
            var existingUserIds = chat.ChatUsers
                .Select(cu => cu.UserId)
                .ToHashSet();

            foreach (var userId in model.AddUserIds.Distinct())
            {
                if (existingUserIds.Contains(userId))
                    continue;

                chat.ChatUsers.Add(new ChatUserEntity
                {
                    UserId = userId,
                    IsAdmin = false
                });
            }
        }

        if (model.RemoveUserIds?.Any() == true)
        {
            var usersToRemove = chat.ChatUsers
                .Where(cu =>
                    model.RemoveUserIds.Contains(cu.UserId) &&
                    cu.UserId != currentUserId)
                .ToList();

            foreach (var cu in usersToRemove)
            {
                chat.ChatUsers.Remove(cu);
            }
        }

        if (!chat.ChatUsers.Any(cu => cu.IsAdmin))
            throw new InvalidOperationException("Chat must have at least one admin");

        await context.SaveChangesAsync();
    }
}
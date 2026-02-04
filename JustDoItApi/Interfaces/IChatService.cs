using JustDoItApi.Models.Chat;

namespace JustDoItApi.Interfaces;

public interface IChatService
{
    Task<long> CreateChatAsync(ChatCreateModel model);
    Task<ChatMessageModel> SendMessageAsync(SendMessageModel model);
    Task<List<ChatTypeItemModel>> GetAllTypes();
    Task<List<UserShortModel>> GetAllUsersAsync();
    Task<List<ChatListItemModel>> GetMyChatsAsync();
    Task<bool> IsUserInChat(long chatId, long userId);
    Task<List<ChatMessageModel>> GetChatMessagesAsync(long chatId);
    Task<bool> AmIAdminAsync(long chatId);
}
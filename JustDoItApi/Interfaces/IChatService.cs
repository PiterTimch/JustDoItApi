using JustDoItApi.Models.Chat;

namespace JustDoItApi.Interfaces;

public interface IChatService
{
    Task<long> CreateChatAsync(ChatCreateModel model);
    Task<ChatMessageModel> SendMessageAsync(SendMessageModel model);
    Task<List<ChatTypeItemModel>> GetAllTypes();
}
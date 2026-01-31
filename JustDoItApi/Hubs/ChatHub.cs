using JustDoItApi.Interfaces;
using JustDoItApi.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JustDoItApi.Hubs;

[Authorize]
public class ChatHub(IChatService chatService) : Hub
{
    public async Task SendMessage(SendMessageModel model)
    {
        var messageDto = await chatService.SendMessageAsync(model);

        await Clients.Group($"chat-{model.ChatId}")
            .SendAsync("ReceiveMessage", messageDto);
    }

    public async Task JoinChat(long chatId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            $"chat-{chatId}"
        );
    }

    public async Task LeaveChat(long chatId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            $"chat-{chatId}"
        );
    }
}
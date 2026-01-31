using AutoMapper;
using JustDoItApi.Entities.Chat;
using JustDoItApi.Models.Chat;

namespace JustDoItApi.Mapper;

public class ChatMapper : Profile
{
    public ChatMapper()
    {
        CreateMap<ChatMessageEntity, ChatMessageModel>();
        CreateMap<ChatTypeEntity, ChatTypeItemModel>();
    }
}
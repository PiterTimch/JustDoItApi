using AutoMapper;
using JustDoItApi.Entities.Chat;
using JustDoItApi.Models.Chat;

namespace JustDoItApi.Mapper;

public class ChatMapper : Profile
{
    public ChatMapper()
    {
        CreateMap<ChatCreateModel, ChatEntity>()
            .ForMember(dest => dest.ChatUsers, opt => opt.MapFrom(
                (src, _, _, ctx) =>
                {
                    var currentUserId = (long)ctx.Items["UserId"];

                    return src.UserIds
                        .Append(currentUserId)
                        .Distinct()
                        .Select(id => new ChatUserEntity
                        {
                            UserId = id,
                            IsAdmin = id == currentUserId
                        }).ToList();
                }));

        CreateMap<ChatMessageEntity, ChatMessageModel>();
        CreateMap<ChatTypeEntity, ChatTypeItemModel>();
    }
}
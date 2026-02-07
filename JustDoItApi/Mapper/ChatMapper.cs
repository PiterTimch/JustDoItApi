using AutoMapper;
using JustDoItApi.Entities.Chat;
using JustDoItApi.Entities.Identity;
using JustDoItApi.Models.Chat;

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
                        })
                        .ToList();
                }));

        CreateMap<ChatEditModel, ChatEntity>()
        .ForMember(dest => dest.ChatUsers, opt => opt.Ignore())
        .ForAllMembers(opt =>
            opt.Condition((src, dest, srcMember) => srcMember != null));


        CreateMap<ChatTypeEntity, ChatTypeItemModel>();

        CreateMap<UserEntity, UserShortModel>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

        CreateMap<ChatEntity, ChatListItemModel>()
            .ForMember(dest => dest.ChatId,
                opt => opt.MapFrom(src => src.Id));

        CreateMap<ChatMessageEntity, ChatMessageModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
            .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.Image));
    }
}

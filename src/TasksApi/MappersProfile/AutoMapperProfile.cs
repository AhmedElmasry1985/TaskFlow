using AutoMapper;
using TasksApi.DTOs;

namespace TasksApi.MappersProfile;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        //from Task to TaskDto
        CreateMap<Models.Task, TaskDto>()
            .ForMember(dest => dest.CreatorUserId, opt => opt.MapFrom(src => src.CreatorUser.ExternalId))
            .ForMember(dest => dest.AssignedUserId, opt => opt.MapFrom(src => src.AssignedUser.ExternalId));

        //from Note to NoteDto
        CreateMap<Models.Note, NoteDto>()
            .ForMember(dest => dest.CreatorUserId, opt => opt.MapFrom(src => src.CreatorUser.ExternalId));
        //from RegisterUserDtoRequest to User
        CreateMap<RegisterUserDtoRequest, Models.User>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
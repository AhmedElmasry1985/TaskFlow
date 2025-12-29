using AutoMapper;
using TasksApi.DTOs;

namespace TasksApi.MappersProfile;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        //from Task to TaskDto
        CreateMap<Models.Task, TaskDto>();
        //from Note to NoteDto
        CreateMap<Models.Note, NoteDto>();
        //from RegisterUserDtoRequest to User
        CreateMap<RegisterUserDtoRequest, Models.User>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
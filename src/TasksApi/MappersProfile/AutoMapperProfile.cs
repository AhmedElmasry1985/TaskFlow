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
    }
}
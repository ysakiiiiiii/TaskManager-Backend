using AutoMapper;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.DTOs.TaskAssignment;
using TaskManagerBackend.Models.Domain;


namespace TaskManagerBackend.Mappings
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<TaskItem, TaskDto>().ReverseMap();
            CreateMap<TaskAssignment, TaskAssignmentDto>().ReverseMap();
            CreateMap<CheckList, CheckListDto>().ReverseMap();
            CreateMap<Attachment, AttachmentDto>().ReverseMap();


            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }

}

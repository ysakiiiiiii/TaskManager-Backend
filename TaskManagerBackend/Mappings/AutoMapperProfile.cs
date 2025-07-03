using AutoMapper;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.DTOs.Comment;
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
            CreateMap<AddTaskRequestDto, TaskItem>()
                .ForMember(dest => dest.CheckListItems, opt => opt.MapFrom(src => src.ChecklistItems))
                .ForMember(dest => dest.AssignedUsers, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (dest.CheckListItems != null)
                    {
                        foreach (var item in dest.CheckListItems)
                        {
                            item.IsCompleted = false;
                        }
                    }
                });
            CreateMap<UpdateTaskRequestDto, TaskItem>()
                .ForMember(dest => dest.CheckListItems, opt => opt.Ignore()) 
                .ForMember(dest => dest.AssignedUsers, opt => opt.Ignore())  
                .ForMember(dest => dest.DateModified, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore());
            
            
            CreateMap<CheckListDto, CheckList>()
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.TaskId, opt => opt.Ignore());

            CreateMap<TaskAssignment, TaskAssignmentDto>().ReverseMap();
            CreateMap<CheckList, CheckListDto>().ReverseMap();
            CreateMap<UpdateChecklistItemDto, CheckList>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())         
            .ForMember(dest => dest.TaskId, opt => opt.Ignore());  

            CreateMap<Attachment, AttachmentDto>().ReverseMap();

            CreateMap<AddCategoryRequestDto, Category>();
            CreateMap<UpdateCategoryRequestDto, Category>();
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<CreateCommentRequestDto, Comment>();

            CreateMap<UpdateCommentRequestDto, Comment>()
                .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UploadAttachmentRequestDto, Attachment>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.File.ContentType))
                .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => Path.GetExtension(src.File.FileName).ToLowerInvariant()))
                .ForMember(dest => dest.FileSizeInBytes, opt => opt.MapFrom(src => src.File.Length))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedById, opt => opt.Ignore())
                .ForMember(dest => dest.DateUploaded, opt => opt.Ignore());

           CreateMap<CheckListDto, CheckList>()
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.TaskId, opt => opt.Ignore());   


        }
    }
}

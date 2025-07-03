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

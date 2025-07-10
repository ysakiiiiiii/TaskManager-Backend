using AutoMapper;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.Category;
using TaskManagerBackend.DTOs.CheckList;
using TaskManagerBackend.DTOs.Comment;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.DTOs.TaskAssignment;
using TaskManagerBackend.DTOs.User;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Mappings
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            // TaskItem → TaskDto
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.CreatedByName,
                    opt => opt.MapFrom(src => src.CreatedBy != null
                        ? src.CreatedBy.FirstName + " " + src.CreatedBy.LastName
                        : string.Empty))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.PriorityName,
                    opt => opt.MapFrom(src => src.Priority != null ? src.Priority.Name : null))
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null))
                .ForMember(dest => dest.AssignedUsers,
                    opt => opt.MapFrom(src => src.AssignedUsers))
                .ForMember(dest => dest.ChecklistItems,
                    opt => opt.MapFrom(src => src.CheckListItems))
                .ForMember(dest => dest.Attachments,
                    opt => opt.MapFrom(src => src.Attachments))
                .ForMember(dest => dest.Comments,
                    opt => opt.MapFrom(src => src.Comments));

            // AddTaskRequestDto → TaskItem
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

            // UpdateTaskRequestDto → TaskItem
            CreateMap<UpdateTaskRequestDto, TaskItem>()
                .ForMember(dest => dest.CheckListItems, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedUsers, opt => opt.Ignore())
                .ForMember(dest => dest.DateModified, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore());

            // TaskAssignment → AssignedUserDto
            CreateMap<TaskAssignment, AssignedUserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

            // Checklist mappings
            CreateMap<CheckListDto, CheckList>()
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.TaskId, opt => opt.Ignore());
            CreateMap<CheckList, CheckListDto>().ReverseMap();

            CreateMap<UpdateChecklistItemDto, CheckList>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore());

            // Attachment mappings
            CreateMap<Attachment, AttachmentDto>().ReverseMap();

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

            // Category
            CreateMap<AddCategoryRequestDto, Category>();
            CreateMap<UpdateCategoryRequestDto, Category>();
            CreateMap<Category, CategoryDto>().ReverseMap();

            // Comment mappings
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<CreateCommentRequestDto, Comment>();
            CreateMap<UpdateCommentRequestDto, Comment>()
                .ForMember(dest => dest.DateUpdated, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());


        }
    }
}

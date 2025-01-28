using AutoMapper;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Application.Requests.Attachments;
using MReminders.API.Application.Requests.Reminders;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Application.Responses.Reminders;
using MReminders.API.Domain.Entities;
using MReminders.API.Domain.Identity;

namespace MReminders.API.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reminder, ReminderResponse>()
            .ForMember(dest => dest.ReminderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ReminderName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ReminderDescription, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ReminderLocation, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.ReminderDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.OverDue, opt => opt.MapFrom(src => src.OverDue))
            .ForMember(dest => dest.Done, opt => opt.MapFrom(src => src.Done))
            .ForPath(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments));

        CreateMap<AddReminderRequest, Reminder>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

        CreateMap<EditReminderRequest, Reminder>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.Done, opt => opt.MapFrom(src => src.Done))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

        CreateMap<Attachment, AttachmentResponse>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.ReminderId, opt => opt.MapFrom(src => src.ReminderId))
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.AttachmentId, opt => opt.MapFrom(src => src.Id));

        CreateMap<AddAttachmentRequest, Attachment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.ReminderId, opt => opt.MapFrom(src => src.ReminderId));

        CreateMap<EditAttachmentRequest, Attachment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.ReminderId, opt => opt.MapFrom(src => src.ReminderId));

        CreateMap<AppUser, UserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
            .ForPath(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(x => new { x.Name }).ToList()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<RegisterAccountRequest, AppUser>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForPath(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => new AppRole { Name = r }).ToList()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));


        CreateMap<(string id, string fullName, string email, string phoneNumber, IList<string> roles), AppUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
            .ForPath(dest => dest.Roles, opt => opt.MapFrom(src => src.roles.Select(r => new AppRole { Name = r })))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.fullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.phoneNumber));
    }
}

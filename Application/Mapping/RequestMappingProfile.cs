using Application.Dtos.Request;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class RequestMappingProfile:Profile
{
    public RequestMappingProfile()
    {
        CreateMap<UserRequest, UserRequestDto>()
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Topic))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<AttachmentUser, AttachmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath));
    }
}
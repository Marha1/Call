using Application.Dtos.AuthDtos;
using Application.Dtos.Request;
using AutoMapper;
using Domain.Models;
using Domain.ValueObject;

namespace Application.Mapping;

public class UserRequestMappingProfile : Profile
{
    public UserRequestMappingProfile()
    {
        CreateMap<UserRequest, UserRequestDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); // Пример преобразования статуса

        CreateMap<UserRequestDto, UserRequest>();

        // Маппинг для создания UserRequest
        CreateMap<UserRequestCreateDto, UserRequest>();
    }
}
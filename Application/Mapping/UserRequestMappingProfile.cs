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

        CreateMap<UpdateUserRequestDto, UserRequest>().ReverseMap();
        
        CreateMap<UserRequestDto, UserRequest>();
        CreateMap<GetUserRequestDto, UserRequest>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))  // Явное указание маппинга для даты
            .ReverseMap()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt));  // И наоборот для другого направления

        // Маппинг для создания UserRequest
        CreateMap<UserRequestCreateDto, UserRequest>();
    }
}
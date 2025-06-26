using AutoMapper;
using Backend.Application.DTOs;
using Backend.Domain.Entities;

namespace Backend.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRole, UserRoleDTO>().ReverseMap();
            CreateMap<VehicleColor, VehicleColorDTO>().ReverseMap();

            // mapping including navigation properties
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles))
                .ForMember(dest => dest.UserPermissions, opt => opt.MapFrom(src => src.UserPermissions))
                .ReverseMap();

            CreateMap<UserPermission, UserPermissionDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.FullName))
                .ForMember(dest => dest.PageUrl, opt => opt.Ignore()); // Skip mapping PageUrl as it's list in DTO but string in entity

            CreateMap<UserPermission, GetUserPermissionDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.FullName)).ReverseMap();

            CreateMap<VehicleDetail, VehicleDetail>()
                .ForMember(dest => dest.VehicleColors, opt => opt.MapFrom(src => src.VehicleColors))
                .ReverseMap();
        }
    }
}

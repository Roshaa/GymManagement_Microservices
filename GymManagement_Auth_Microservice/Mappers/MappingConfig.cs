using AutoMapper;
using GymManagement_Auth_Microservice.DTO_s;
using Microsoft.AspNetCore.Identity;

namespace GymManagement_Auth_Microservice.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {

            CreateMap<RegisterUserDTO, IdentityUser>()
                .ForMember(dest => dest.UserName,     opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email,        opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber,  opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp,    opt => opt.Ignore());

            CreateMap<UserDTO, IdentityUser>()
                .ForMember(dest => dest.Id,           opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName,     opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PhoneNumber,  opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email,        opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp,    opt => opt.Ignore());

            CreateMap<IdentityUser, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));

        }
    }
}

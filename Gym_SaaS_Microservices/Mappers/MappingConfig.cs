using AutoMapper;
using Gym_SaaS_Microservices.DTO_s;
using Gym_SaaS_Microservices.Models;

namespace Gym_SaaS_Microservices.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CreatePromoDTO, Promo>();
            CreateMap<Promo, PromoDTO>();
        }
    }
}

using AutoMapper;
using GymManagement_Promo_Microservice.DTO_s;
using GymManagement_Promo_Microservice.Models;
using GymManagement_Shared_Classes.DTO_s;

namespace GymManagement_Promo_Microservice.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CreatePromoDTO, Promo>();
            CreateMap<Promo, PromoDTO>();
            CreateMap<Promo, PromoAnswerDTO>();
        }
    }
}

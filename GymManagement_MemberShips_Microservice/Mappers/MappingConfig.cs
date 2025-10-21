using AutoMapper;
using GymManagement_MemberShips_Microservice.DTO_s;
using GymManagement_MemberShips_Microservice.Models;
using GymManagement_MembersShips_Microservice.DTO_s;

namespace GymManagement_MemberShips_Microservice.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CreateMemberSubscriptionDTO, MemberSubscription>();
            CreateMap<MemberSubscription, MemberSubscriptionDTO>();
        }
    }
}

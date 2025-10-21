using AutoMapper;
using GymManagement_Members_Microservice.DTO_s;
using GymManagement_Members_Microservice.Models;

namespace GymManagement_Members_Microservice.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CreateMemberDTO, Member>();
            CreateMap<Member, MemberDTO>();
            CreateMap<Member, CreateMemberSubscriptionDTO>()
                .ForMember(c => c.PaymentDay, opt => opt.Ignore())
                .ForMember(c => c.MemberId, opt => opt.MapFrom(m => m.Id));
            CreateMap<Member, ChangeMemberSubscriptionDTO>()
                .ForMember(c => c.MemberId, opt => opt.MapFrom(m => m.Id));
        }
    }
}

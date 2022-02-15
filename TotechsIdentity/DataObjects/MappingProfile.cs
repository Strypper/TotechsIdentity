using AutoMapper;
using Entities;
using System.Linq;

namespace TotechsIdentity.DataObjects
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDTO, User>().ForMember(d => d.Guid, o => o.Ignore());
            CreateMap<User, UserDTO>().ForMember(d => d.Roles, o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role!.Name)));
        }
    }
}

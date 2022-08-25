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

            CreateMap<CreateUserDTO, User>().ForMember(d => d.Guid, o => o.Ignore());

            CreateMap<Role, RoleDTO>();
            CreateMap<RoleDTO, Role>()
                .ForMember(ent => ent.Id, opt => opt.Ignore());

            CreateMap<RoleLevel, RoleLevelDTO>();
            CreateMap<RoleLevelDTO, RoleLevel>();

            CreateMap<Country, CountryDTO>();
            CreateMap<CountryDTO, Country>();
        }
    }
}

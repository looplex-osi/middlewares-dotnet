using AutoMapper;
using Looplex.DotNet.Middlewares.ScimV2.DTOs.Groups;
using Looplex.DotNet.Middlewares.ScimV2.DTOs.Users;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

namespace Looplex.DotNet.Middlewares.ScimV2.Profiles
{
    internal class ScimV2Profile : Profile
    {
        public ScimV2Profile()
        {
            CreateMap<Address, AddressDTO>()
                .ReverseMap();
            CreateMap<Email, EmailDTO>()
                .ReverseMap();
            CreateMap<Entitlement, EntitlementDTO>()
                .ReverseMap();
            CreateMap<UserGroup, UserGroupDTO>()
                .ReverseMap();
            CreateMap<IM, IMDTO>()
                .ReverseMap();
            CreateMap<Name, NameDTO>()
                .ReverseMap();
            CreateMap<PhoneNumber, PhoneNumberDTO>()
                .ReverseMap();
            CreateMap<Photo, PhotoDTO>()
                .ReverseMap();
            CreateMap<Role, RoleDTO>()
                .ReverseMap();
            CreateMap<User, UserDTO>()
                .ReverseMap();
            CreateMap<Group, GroupDTO>()
                .ReverseMap();
            CreateMap<Member, MemberDTO>()
                .ReverseMap();
        }
    }
}
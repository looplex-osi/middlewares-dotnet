using AutoMapper;
using Looplex.DotNet.Middlewares.ScimV2.Dtos.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Dtos.Users;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

namespace Looplex.DotNet.Middlewares.ScimV2.Profiles
{
    internal class ScimV2Profile : Profile
    {
        public ScimV2Profile()
        {
            CreateMap<Address, AddressDto>()
                .ReverseMap();
            CreateMap<Email, EmailDto>()
                .ReverseMap();
            CreateMap<Entitlement, EntitlementDto>()
                .ReverseMap();
            CreateMap<UserGroup, UserGroupDto>()
                .ReverseMap();
            CreateMap<InstantMessaging, InstantMessagingDto>()
                .ReverseMap();
            CreateMap<Name, NameDto>()
                .ReverseMap();
            CreateMap<PhoneNumber, PhoneNumberDto>()
                .ReverseMap();
            CreateMap<Photo, PhotoDto>()
                .ReverseMap();
            CreateMap<Role, RoleDto>()
                .ReverseMap();
            CreateMap<User, UserDto>()
                .ReverseMap();
            CreateMap<Group, GroupDto>()
                .ReverseMap();
            CreateMap<Member, MemberDto>()
                .ReverseMap();
        }
    }
}
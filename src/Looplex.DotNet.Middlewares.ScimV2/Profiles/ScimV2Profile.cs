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
            CreateMap<AddressElement, AddressDto>()
                .ReverseMap();
            CreateMap<EmailElement, EmailDto>()
                .ReverseMap();
            CreateMap<EntitlementElement, EntitlementDto>()
                .ReverseMap();
            CreateMap<GroupElement, UserGroupDto>()
                .ReverseMap();
            CreateMap<ImElement, InstantMessagingDto>()
                .ReverseMap();
            CreateMap<Name, NameDto>()
                .ReverseMap();
            CreateMap<PhoneNumberElement, PhoneNumberDto>()
                .ReverseMap();
            CreateMap<PhotoElement, PhotoDto>()
                .ReverseMap();
            CreateMap<RoleElement, RoleDto>()
                .ReverseMap();
            CreateMap<User, UserDto>()
                .ReverseMap();
            CreateMap<Group, GroupDto>()
                .ReverseMap();
            CreateMap<MemberElement, MemberDto>()
                .ReverseMap();
        }
    }
}
using AutoMapper;
using Looplex.DotNet.Middlewares.OAuth2.DTOs;
using Looplex.DotNet.Middlewares.OAuth2.Entities;

namespace Looplex.DotNet.Middlewares.OAuth2.Profiles
{
    internal class OAuth2Profile : Profile
    {
        public OAuth2Profile()
        {
            CreateMap<Client, ClientDTO>()
                .ReverseMap();
            CreateMap<Client, ClientReadDTO>()
                .ReverseMap();
            CreateMap<Client, ClientWriteDTO>()
                .ReverseMap();
        }
    }
}
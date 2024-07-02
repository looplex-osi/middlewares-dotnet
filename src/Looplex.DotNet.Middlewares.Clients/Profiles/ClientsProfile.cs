using AutoMapper;
using Looplex.DotNet.Middlewares.Clients.DTOs;
using Looplex.DotNet.Middlewares.Clients.Entities;

namespace Looplex.DotNet.Middlewares.Clients.Profiles
{
    internal class ClientsProfile : Profile
    {
        public ClientsProfile()
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
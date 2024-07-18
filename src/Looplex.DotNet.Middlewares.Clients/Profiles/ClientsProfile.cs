using AutoMapper;
using Looplex.DotNet.Middlewares.Clients.Dtos;
using Looplex.DotNet.Middlewares.Clients.Entities.Clients;

namespace Looplex.DotNet.Middlewares.Clients.Profiles
{
    internal class ClientsProfile : Profile
    {
        public ClientsProfile()
        {
            CreateMap<Client, ClientDto>()
                .ReverseMap();
            CreateMap<Client, ClientReadDto>()
                .ReverseMap();
            CreateMap<Client, ClientWriteDto>()
                .ReverseMap();
        }
    }
}
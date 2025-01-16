using AutoMapper;
using Romb.Application.Dtos;
using Romb.Application.Entities;

namespace Romb.Application.Mappers;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<EventEntity, EventOutputDto>();

        CreateMap<EventInputDto, EventEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LocalBudget, opt => opt.Ignore())
            .ForMember(dest => dest.RegionalBudget, opt => opt.Ignore()); 
    }
}

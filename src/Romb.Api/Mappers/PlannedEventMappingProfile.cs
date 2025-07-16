using AutoMapper;
using Romb.Application.Dtos;
using Romb.Application.Entities;

namespace Romb.Application.Mappers;

public class PlannedEventMappingProfile : Profile
{
    public PlannedEventMappingProfile()
    {
        CreateMap<PlannedEventEntity, PlannedEventResponceDto>();

        CreateMap<PlannedEventRequestDto, PlannedEventEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PlannedLocalBudget, opt => opt.Ignore())
            .ForMember(dest => dest.PlannedRegionalBudget, opt => opt.Ignore()); 
    }
}

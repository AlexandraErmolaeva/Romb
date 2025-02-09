using AutoMapper;
using Romb.Application.Dtos;
using Romb.Application.Entities;

namespace Romb.Application.Mappers;

public class PlanedEventMappingProfile : Profile
{
    public PlanedEventMappingProfile()
    {
        CreateMap<PlannedEventEntity, PlannedEventOutputDto>();

        CreateMap<PlannedEventInputDto, PlannedEventEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PlannedLocalBudget, opt => opt.Ignore())
            .ForMember(dest => dest.PlannedRegionalBudget, opt => opt.Ignore()); 
    }
}

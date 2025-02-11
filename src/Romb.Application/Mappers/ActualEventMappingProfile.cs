using AutoMapper;
using Romb.Application.Dtos;
using Romb.Application.Entities;

namespace Romb.Application.Mappers;

public class ActualEventMappingProfile : Profile
{
    public ActualEventMappingProfile()
    {
        CreateMap<ActualEventEntity, ActualEventOutputDto>()
            .ForMember(dest => dest.TargetCode, opt => opt.MapFrom(srt => srt.PlannedEvent.TargetCode));

        CreateMap<ActualEventInputDto, ActualEventEntity>()
            .ForMember(dest => dest.ActualCofinanceRate, opt => opt.Ignore())
            .ForMember(dest => dest.ActualLocalBudget, opt => opt.Ignore())
            .ForMember(dest => dest.ActualRegionalBudget, opt => opt.Ignore())
            .ForMember(dest => dest.PlannedEvent, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}

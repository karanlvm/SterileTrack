using AutoMapper;
using SterileTrack.Application.DTOs;
using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Device, DeviceDto>().ReverseMap();
        CreateMap<StatusHistory, StatusHistoryDto>().ReverseMap();
        CreateMap<SterilizationCycle, SterilizationCycleDto>()
            .ForMember(dest => dest.DeviceIdentifier, opt => opt.MapFrom(src => src.Device.DeviceIdentifier));
    }
}

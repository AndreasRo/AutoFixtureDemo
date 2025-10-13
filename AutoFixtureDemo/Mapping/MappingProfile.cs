using AutoMapper;
using AutoFixtureDemo.Controllers.DTO;
using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WeatherForecast, WeatherForecastDto>()
                .ForMember(dest => dest.TemperatureC, opt => opt.MapFrom(src => src.TemperatureC.Value))
                .ForMember(dest => dest.TemperatureF, opt => opt.MapFrom(src => src.TemperatureF.Value));

            CreateMap<Location, LocationDto>();
        }
    }
}

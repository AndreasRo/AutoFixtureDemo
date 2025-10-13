using AutoMapper;
using AutoFixtureDemo.Controllers.DTO;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Database.Entities;

namespace AutoFixtureDemo.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WeatherForecast, WeatherForecastDto>()
                .ForMember(dest => dest.TemperatureC, opt => opt.MapFrom(src => src.TemperatureC.Value))
                .ForMember(dest => dest.TemperatureF, opt => opt.MapFrom(src => src.TemperatureF.Value))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => $"{src.Summary:G}"));

            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.Value))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Value));

            // Database entity -> domain mappings
            CreateMap<WeatherForecastEntity, WeatherForecast>()
                .ForMember(dest => dest.TemperatureC, opt => opt.MapFrom(src => new Celsius(src.TemperatureC)))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary));

            CreateMap<LocationEntity, Location>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new Text(src.City)))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => new Text(src.Country)))
                .ForMember(dest => dest.Forecasts, opt => opt.MapFrom(src => src.Forecasts));
        }
    }
}

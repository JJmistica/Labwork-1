using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class SnackMappings : Profile
    {
        public SnackMappings()
        {
            CreateMap<SnackEntity, SnackDto>().ReverseMap();
            CreateMap<SnackEntity, SnackUpdateDto>().ReverseMap();
            CreateMap<SnackEntity, SnackCreateDto>().ReverseMap();
        }
    }
}

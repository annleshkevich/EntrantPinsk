using AutoMapper;
using EntrantPinsk.Common.DTOs;
using EntrantPinsk.Model.DatabaseModels;

namespace EntrantPinsk.Common.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Direction, DirectionDto>().ReverseMap();
            CreateMap<Education, EducationDto>().ReverseMap();
            CreateMap<EducationalInstitution, InstitutionDto>().ReverseMap();
            CreateMap<Specialty, SpecialtyDto>().ReverseMap();

        }
    }
}

using AutoMapper;
using EntrantPinsk.BusinessLogic.Services.Interfaces;
using EntrantPinsk.Common.DTOs;
using EntrantPinsk.Model;
using Microsoft.EntityFrameworkCore;

namespace EntrantPinsk.BusinessLogic.Services.Implementations
{
    public class EducationService : IEducationService
    {
        private readonly ApplicationContext _db;
        private readonly IMapper _mapper;
        public EducationService(ApplicationContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IEnumerable<EducationDto> Get() 
        {
            return _mapper.Map<List<EducationDto>>(_db.Educations.AsNoTracking().ToList()); 
        }
        public EducationDto Get(int id)
        {
            return _mapper.Map<EducationDto>(_db.Educations.FirstOrDefault(u => u.Id == id)!);
        }
    }
}

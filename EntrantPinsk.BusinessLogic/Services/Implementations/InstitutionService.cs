using AutoMapper;
using EntrantPinsk.BusinessLogic.Services.Interfaces;
using EntrantPinsk.Common.DTOs;
using EntrantPinsk.Model;
using Microsoft.EntityFrameworkCore;

namespace EntrantPinsk.BusinessLogic.Services.Implementations
{
    public class InstitutionService : IInstitutionService
    {
        private readonly ApplicationContext _db;
        private readonly IMapper _mapper;
        public InstitutionService(ApplicationContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IEnumerable<InstitutionDto> Get() =>
            _mapper.Map<List<InstitutionDto>>(_db.EducationalInstitutions.AsNoTracking().ToList());
        public InstitutionDto Get(int id) =>
            _mapper.Map<InstitutionDto>(_db.EducationalInstitutions.FirstOrDefault(u => u.Id == id)!);
    }
}

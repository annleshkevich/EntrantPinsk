using AutoMapper;
using EntrantPinsk.BusinessLogic.Services.Interfaces;
using EntrantPinsk.Common.DTOs;
using EntrantPinsk.Model;
using Microsoft.EntityFrameworkCore;

namespace EntrantPinsk.BusinessLogic.Services.Implementations
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ApplicationContext _db;
        private readonly IMapper _mapper;
        public SpecialtyService(ApplicationContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IEnumerable<SpecialtyDto> Get() =>
            _mapper.Map<List<SpecialtyDto>>(_db.Specialties.AsNoTracking().ToList());
        public SpecialtyDto Get(int id) =>
            _mapper.Map<SpecialtyDto>(_db.Specialties.FirstOrDefault(u => u.Id == id)!);
    }
}

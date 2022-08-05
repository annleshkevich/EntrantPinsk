using AutoMapper;
using EntrantPinsk.BusinessLogic.Services.Interfaces;
using EntrantPinsk.Common.DTOs;
using EntrantPinsk.Model;
using Microsoft.EntityFrameworkCore;

namespace EntrantPinsk.BusinessLogic.Services.Implementations
{
    public class DirectionService : IDirectionService
    {
        private readonly ApplicationContext _db;
        private readonly IMapper _mapper;
        public DirectionService(ApplicationContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IEnumerable<DirectionDto> Get() =>
            _mapper.Map<List<DirectionDto>>(_db.Directions.AsNoTracking().ToList());
        public DirectionDto Get(int id) =>
            _mapper.Map<DirectionDto>(_db.Directions.FirstOrDefault(u => u.Id == id)!);
    }
}

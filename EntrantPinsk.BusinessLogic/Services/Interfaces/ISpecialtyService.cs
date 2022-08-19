using EntrantPinsk.Common.DTOs;

namespace EntrantPinsk.BusinessLogic.Services.Interfaces
{
    public interface ISpecialtyService
    {
        IEnumerable<SpecialtyDto> Get();
        SpecialtyDto Get(int id);
    }
}

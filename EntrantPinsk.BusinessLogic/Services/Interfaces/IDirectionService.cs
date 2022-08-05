using EntrantPinsk.Common.DTOs;

namespace EntrantPinsk.BusinessLogic.Services.Interfaces
{
    public interface IDirectionService
    {
        IEnumerable<DirectionDto> Get();
        DirectionDto Get(int id);
    }
}
